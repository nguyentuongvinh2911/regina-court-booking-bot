using System;
using System.Globalization;
using System.Threading.Tasks;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Models;
using ReginaCourtBookingBot.Pages;

namespace ReginaCourtBookingBot.Services
{
    public class BookingService
    {
        private readonly ReserveOptionsPage _reserveOptionsPage;
        private readonly AppSettings _appSettings;

        public BookingService(ReserveOptionsPage reserveOptionsPage, AppSettings appSettings)
        {
            _reserveOptionsPage = reserveOptionsPage;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Full booking flow based on the recorded Regina flow:
        ///   1. Navigate → 2. Choose court type → 3. Open sign-in
        ///   4. Login → 5. Pick date → 6. Pick slot
        ///   7. Fill details/waivers → 8. Reserve
        ///
        /// Set AppSettings.DryRun = true to stop before the final Reserve click.
        /// </summary>
        public async Task<bool> CreateBookingAsync(BookingRequest bookingRequest)
        {
            if (bookingRequest == null)
                throw new ArgumentNullException(nameof(bookingRequest));

            var selectors = _appSettings.Selectors;

            // Step 0 — Navigate to the landing page
            await _reserveOptionsPage.NavigateToReserveOptionsAsync(_appSettings.BaseUrl);

            // Step 1 — Click court type tile (Badminton / Tennis)
            await _reserveOptionsPage.SelectCourtTypeAsync(bookingRequest.CourtType, selectors);

            // Step 2 — Open sign-in if the site presents it
            await _reserveOptionsPage.OpenSignInIfPresentAsync(selectors);

            // Step 3 — Log in if prompted
            await _reserveOptionsPage.LoginIfRequiredAsync(
                _appSettings.Username, _appSettings.Password, selectors);

            // Step 4 — Pick booking date
            await _reserveOptionsPage.PickDateAsync(bookingRequest.Date, selectors);

            // Step 5 — Pick requested slot
            var slotFound = await _reserveOptionsPage.SelectSlotAsync(bookingRequest, selectors);
            if (!slotFound)
            {
                Console.WriteLine("No available court slot found for the requested date/time.");
                return false;
            }

            // Step 6 — Fill event/player details and accept waivers.
            // Gate the hot click ("1 Resource(s), 1 Booking(s)") at an exact local time.
            await _reserveOptionsPage.FillBookingDetailsAsync(
                bookingRequest,
                selectors,
                async () => await WaitUntilReviewClickTimeAsync());

            // ---- Dry run stops here ----
            if (_appSettings.DryRun)
            {
                await _reserveOptionsPage.TakeScreenshotAsync("booking-dry-run.png");
                Console.WriteLine("DryRun=true — stopping before the final reserve click.");
                return true;
            }

            // Step 7 — Final reserve click
            return await _reserveOptionsPage.ReserveAsync(selectors);
        }

        private async Task WaitUntilReviewClickTimeAsync()
        {
            if (string.IsNullOrWhiteSpace(_appSettings.ReviewButtonClickAtLocalTime))
            {
                return;
            }

            if (!TimeSpan.TryParseExact(
                _appSettings.ReviewButtonClickAtLocalTime,
                new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss", @"H\:mm\:ss" },
                CultureInfo.InvariantCulture,
                out var targetTime))
            {
                throw new InvalidOperationException("AppSettings:ReviewButtonClickAtLocalTime must be in HH:mm or HH:mm:ss format.");
            }

            var now = DateTime.Now;
            var targetToday = now.Date.Add(targetTime);

            if (now < targetToday)
            {
                var delay = targetToday - now;
                Console.WriteLine($"Waiting until {targetToday:HH:mm:ss} to click review/bookings button.");
                await Task.Delay(delay);
                return;
            }

            var lateBy = now - targetToday;
            if (_appSettings.StrictReviewButtonClickTime && lateBy > TimeSpan.FromSeconds(1))
            {
                throw new InvalidOperationException(
                    $"Missed exact review click time {targetToday:HH:mm:ss} by {lateBy.TotalSeconds:F2}s. " +
                    "Set StrictReviewButtonClickTime=false to allow immediate click when late.");
            }

            Console.WriteLine($"Review click target already reached ({targetToday:HH:mm:ss}); clicking now.");
        }
    }
}
