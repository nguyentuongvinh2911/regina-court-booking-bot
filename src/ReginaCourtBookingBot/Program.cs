using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Models;
using ReginaCourtBookingBot.Pages;
using ReginaCourtBookingBot.Services;

namespace ReginaCourtBookingBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("secrets.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>(optional: true)
                .AddEnvironmentVariables(prefix: "BOOKINGBOT_")
                .Build();

            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = appSettings.Headless,
                SlowMo = appSettings.SlowMoMilliseconds
            });

            var page = await browser.NewPageAsync();
            var reserveOptionsPage = new ReserveOptionsPage(page);
            var bookingService = new BookingService(reserveOptionsPage, appSettings);

            await WaitUntilScheduledStartAsync(appSettings.RunAtLocalTime);

            var bookingRequest = BuildBookingRequest(configuration);

            Console.WriteLine($"Starting booking flow. DryRun={appSettings.DryRun}, Headless={appSettings.Headless}");
            var bookingResult = await bookingService.CreateBookingAsync(bookingRequest);
            Console.WriteLine($"Booking flow completed. Success={bookingResult}");

            await browser.CloseAsync();
        }

        private static BookingRequest BuildBookingRequest(IConfiguration configuration)
        {
            var dateValue = configuration["BookingRequest:Date"];
            var dateOffsetDays = int.TryParse(configuration["BookingRequest:DateOffsetDays"], out var parsedOffset)
                ? parsedOffset
                : 3;

            var slotFallbacks = configuration.GetSection("BookingRequest:SlotFallbacks").Get<List<string>>() ?? new List<string>();

            return new BookingRequest
            {
                Date = DateTime.TryParse(dateValue, out var parsedDate) ? parsedDate : DateTime.Today.AddDays(dateOffsetDays),
                CourtType = configuration["BookingRequest:CourtType"] ?? "Badminton",
                SlotLabel = configuration["BookingRequest:SlotLabel"] ?? string.Empty,
                SlotFallbacks = slotFallbacks,
                EventName = configuration["BookingRequest:EventName"] ?? "1",
                Player2FullName = configuration["BookingRequest:Player2FullName"] ?? string.Empty
            };
        }

        private static async Task WaitUntilScheduledStartAsync(string runAtLocalTime)
        {
            if (!TryParseRunTime(runAtLocalTime, out var scheduledTime))
            {
                return;
            }

            var now = DateTime.Now;
            var scheduledStart = now.Date.Add(scheduledTime);

            if (scheduledStart <= now)
            {
                scheduledStart = scheduledStart.AddDays(1);
            }

            var delay = scheduledStart - now;
            Console.WriteLine($"Waiting until local time {scheduledStart:yyyy-MM-dd HH:mm:ss} before starting the booking flow.");
            await Task.Delay(delay);
        }

        private static bool TryParseRunTime(string runAtLocalTime, out TimeSpan scheduledTime)
        {
            if (string.IsNullOrWhiteSpace(runAtLocalTime))
            {
                scheduledTime = default;
                return false;
            }

            if (!TimeSpan.TryParseExact(runAtLocalTime, new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out scheduledTime))
            {
                throw new InvalidOperationException("AppSettings:RunAtLocalTime must be in HH:mm or HH:mm:ss format.");
            }

            return true;
        }
    }
}