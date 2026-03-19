using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Models;
using ReginaCourtBookingBot.Pages;

namespace ReginaCourtBookingBot.Services
{
    public class BookingRunner
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly ILogger<BookingRunner> _logger;

        public BookingRunner(IConfiguration configuration, AppSettings appSettings, ILogger<BookingRunner> logger)
        {
            _configuration = configuration;
            _appSettings = appSettings;
            _logger = logger;
        }

        public async Task<bool> RunOnceAsync(bool waitForScheduledStart, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (waitForScheduledStart)
            {
                await WaitUntilScheduledStartAsync(cancellationToken);
            }

            var bookingRequest = BuildBookingRequest();

            _logger.LogInformation("Starting booking flow. DryRun={DryRun}, Headless={Headless}", _appSettings.DryRun, _appSettings.Headless);

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = _appSettings.Headless,
                SlowMo = _appSettings.SlowMoMilliseconds
            });

            try
            {
                var page = await browser.NewPageAsync();
                var reserveOptionsPage = new ReserveOptionsPage(page);
                var bookingService = new BookingService(reserveOptionsPage, _appSettings);

                var bookingResult = await bookingService.CreateBookingAsync(bookingRequest);
                _logger.LogInformation("Booking flow completed. Success={Success}", bookingResult);
                return bookingResult;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }

        private BookingRequest BuildBookingRequest()
        {
            var dateValue = _configuration["BookingRequest:Date"];
            var dateOffsetDays = int.TryParse(_configuration["BookingRequest:DateOffsetDays"], out var parsedOffset)
                ? parsedOffset
                : 3;

            var slotFallbacks = _configuration.GetSection("BookingRequest:SlotFallbacks").Get<List<string>>() ?? new List<string>();

            return new BookingRequest
            {
                Date = DateTime.TryParse(dateValue, out var parsedDate) ? parsedDate : DateTime.Today.AddDays(dateOffsetDays),
                CourtType = _configuration["BookingRequest:CourtType"] ?? "Badminton",
                SlotLabel = _configuration["BookingRequest:SlotLabel"] ?? string.Empty,
                SlotFallbacks = slotFallbacks,
                EventName = _configuration["BookingRequest:EventName"] ?? "1",
                Player2FullName = _configuration["BookingRequest:Player2FullName"] ?? string.Empty
            };
        }

        private async Task WaitUntilScheduledStartAsync(CancellationToken cancellationToken)
        {
            if (!ScheduleCalculator.TryParseRunTime(_appSettings.RunAtLocalTime, out var scheduledTime))
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
            _logger.LogInformation("Waiting until local time {ScheduledStart:yyyy-MM-dd HH:mm:ss} before starting the booking flow.", scheduledStart);
            await Task.Delay(delay, cancellationToken);
        }
    }
}
