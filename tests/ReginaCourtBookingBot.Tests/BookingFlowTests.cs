using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Models;
using ReginaCourtBookingBot.Pages;
using ReginaCourtBookingBot.Services;
using Xunit;

namespace ReginaCourtBookingBot.Tests
{
    public class BookingFlowTests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private ReserveOptionsPage _reserveOptionsPage;
        private AppSettings _appSettings;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            _page = await _browser.NewPageAsync();
            _reserveOptionsPage = new ReserveOptionsPage(_page);
            // Load credentials from user-secrets (same as Program.cs) so right-click
            // "Run Tests" in VS Code works without manually setting env vars.
            var config = new ConfigurationBuilder()
                .AddUserSecrets<BookingFlowTests>(optional: true)
                .AddEnvironmentVariables(prefix: "BOOKINGBOT_")
                .Build();

            _appSettings = new AppSettings
            {
                BaseUrl = "https://ca.apm.activecommunities.com/regina/Reserve_Options",
                Username = config["AppSettings:Username"] ?? string.Empty,
                Password = config["AppSettings:Password"] ?? string.Empty,
                DryRun = true,
                Headless = false,
                RunAtLocalTime = string.Empty,
                // Fire the review-button click immediately in tests (no wait, no strict gate)
                ReviewButtonClickAtLocalTime = string.Empty,
                StrictReviewButtonClickTime = false
            };
        }

        [Fact]
        public async Task ShouldNavigateToReserveOptionsPage()
        {
            await _reserveOptionsPage.NavigateToReserveOptionsAsync(_appSettings.BaseUrl);
            Assert.NotNull(_page.Url);
            Assert.Contains("reservation", _page.Url);
        }

        [Fact]
        public async Task ShouldSearchAvailabilityWithoutError()
        {
            var bookingRequest = new BookingRequest
            {
                Date = DateTime.Today.AddDays(7),
                CourtType = "Badminton",
                SlotLabel = "FH - Badminton Court 1 7:00 PM - 8:00 PM Available",
                EventName = "1",
                Player2FullName = "Test Player"
            };

            await _reserveOptionsPage.NavigateToReserveOptionsAsync(_appSettings.BaseUrl);

            // Step 1: click court type tile
            await _reserveOptionsPage.SelectCourtTypeAsync(bookingRequest.CourtType, _appSettings.Selectors);

            // Step 2-4: open sign-in. Full login/date selection only runs when
            // credentials are provided through environment variables.
            await _reserveOptionsPage.OpenSignInIfPresentAsync(_appSettings.Selectors);

            if (!HasLiveCredentials())
            {
                return;
            }

            await _reserveOptionsPage.LoginIfRequiredAsync(_appSettings.Username, _appSettings.Password, _appSettings.Selectors);
            await _reserveOptionsPage.PickDateAsync(bookingRequest.Date, _appSettings.Selectors);
        }

        [Fact]
        public async Task BookingServiceShouldCompleteWithDryRun()
        {
            if (!HasLiveCredentials())
            {
                return;
            }

            var bookingService = new BookingService(_reserveOptionsPage, _appSettings);
            var bookingRequest = new BookingRequest
            {
                Date = new DateTime(2026, 3, 20), // Friday March 20th
                CourtType = "Badminton",
                SlotLabel = "FH - Badminton Court 2 7:00 AM - 8:00 AM Available",
                EventName = "1",
                Player2FullName = "Test Player"
            };

            // DryRun=true returns true after Step 3 (slot selection) without
            // actually logging in or submitting the reservation.
            var result = await bookingService.CreateBookingAsync(bookingRequest);
            Assert.True(result);
        }

        public async Task DisposeAsync()
        {
            if (_page != null) await _page.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        private bool HasLiveCredentials()
        {
            return !string.IsNullOrWhiteSpace(_appSettings.Username)
                && !string.IsNullOrWhiteSpace(_appSettings.Password);
        }
    }
}
