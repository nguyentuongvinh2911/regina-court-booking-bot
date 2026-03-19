using Microsoft.Playwright;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ReginaCourtBookingBot.Pages
{
    /// <summary>
    /// Page Object for the Regina Active Communities booking flow.
    ///
    /// HOW TO RECORD REAL SELECTORS WITH CODEGEN:
    ///   1. Build the project first:
    ///        dotnet build
    ///   2. Launch the Playwright Inspector / code recorder:
    ///        pwsh bin/Debug/net8.0/playwright.ps1 codegen --target csharp-nunit --output RecordedBooking.cs "https://ca.apm.activecommunities.com/regina/Reserve_Options"
    ///   3. Walk through your full booking in the browser that opens.
    ///   4. When you close the browser, RecordedBooking.cs contains every
    ///      locator and action Playwright recorded.
    ///   5. Copy the locator strings into AppSettings.Selectors (or appsettings.json).
    ///
    /// EXAMPLE — codegen typically outputs lines like:
    ///   await page.GetByText("Badminton Bookings").ClickAsync();
    ///   await page.GetByLabel("Start Time").FillAsync("18:00");
    ///   await page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
    ///
    /// Map those locator strings to the matching properties in BookingSelectors.
    /// </summary>
    public class ReserveOptionsPage
    {
        private readonly IPage _page;

        public ReserveOptionsPage(IPage page)
        {
            _page = page;
        }

        // ------------------------------------------------------------------ //
        // Step 0 — Navigate
        // ------------------------------------------------------------------ //

        public async Task NavigateToReserveOptionsAsync(string url)
        {
            await _page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
        }

        // ------------------------------------------------------------------ //
        // Step 1 — Select court type (Badminton / Tennis tile)
        // ------------------------------------------------------------------ //

        public async Task SelectCourtTypeAsync(string courtType, BookingSelectors selectors)
        {
            var link = courtType.Equals("Tennis", StringComparison.OrdinalIgnoreCase)
                ? selectors.TennisBookingsLink
                : selectors.BadmintonBookingsLink;

            var locator = _page.Locator(link);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15_000
            });
            await locator.ClickAsync();

            Console.WriteLine($"[Step 1] Clicked court type: {courtType}");
        }

        public async Task OpenSignInIfPresentAsync(BookingSelectors selectors)
        {
            var signInLink = _page.Locator(selectors.SignInNowLink);
            if (await signInLink.CountAsync() == 0)
            {
                Console.WriteLine("[Step 2] Sign-in link not shown.");
                return;
            }

            await signInLink.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Console.WriteLine("[Step 2] Opened sign-in form.");
        }

        public async Task LoginIfRequiredAsync(string username, string password, BookingSelectors selectors)
        {
            var loginNameInput = _page.Locator(selectors.LoginNameInput);
            if (await loginNameInput.CountAsync() == 0)
            {
                Console.WriteLine("[Step 3] Login not required (already authenticated).");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("Username/password are required when the site prompts for sign-in.");
            }

            Console.WriteLine("[Step 3] Login gate detected — signing in.");
            await loginNameInput.FillAsync(username);
            var passwordInput = _page.Locator(selectors.PasswordInput);
            await passwordInput.FillAsync(password);
            await passwordInput.PressAsync("Enter");
            // Wait for the date picker — confirms login succeeded on this SPA.
            await _page.WaitForSelectorAsync(selectors.DatePicker, new PageWaitForSelectorOptions { Timeout = 30_000 });
        }

        public async Task PickDateAsync(DateTime bookingDate, BookingSelectors selectors)
        {
            var datePicker = _page.Locator(selectors.DatePicker);
            await datePicker.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15_000
            });
            await datePicker.ClickAsync();

            var dayLabel = bookingDate.ToString("MMM d,", CultureInfo.InvariantCulture);
            var dayLocator = _page.GetByRole(AriaRole.Region, new() { Name = dayLabel });

            if (await dayLocator.CountAsync() == 0)
            {
                dayLocator = _page.GetByRole(AriaRole.Gridcell, new() { Name = dayLabel });
            }

            if (await dayLocator.CountAsync() == 0)
            {
                throw new InvalidOperationException($"Could not find calendar day '{dayLabel}'.");
            }

            await dayLocator.First.ClickAsync();
            // Calendar is AJAX-driven — wait for slots to render instead of NetworkIdle.
            await _page.WaitForSelectorAsync("role=gridcell", new PageWaitForSelectorOptions { Timeout = 15_000 });
            Console.WriteLine($"[Step 4] Picked booking date: {bookingDate:yyyy-MM-dd}");
        }

        public async Task<bool> SelectSlotAsync(BookingRequest request, BookingSelectors selectors)
        {
            // Build the priority list: primary slot first, then each fallback in order.
            var candidates = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(request.SlotLabel))
                candidates.Add(request.SlotLabel);
            if (request.SlotFallbacks != null)
                candidates.AddRange(request.SlotFallbacks);

            foreach (var label in candidates)
            {
                var locator = _page.GetByRole(AriaRole.Gridcell, new() { Name = label });
                if (await locator.CountAsync() > 0)
                {
                    await locator.ClickAsync();
                    Console.WriteLine($"[Step 5] Selected slot: {label}");
                    return true;
                }
                Console.WriteLine($"[Step 5] Slot not available, skipping: {label}");
            }

            // Last resort: click the first visible gridcell if no named slot matched.
            if (candidates.Count == 0)
            {
                var fallback = _page.Locator(selectors.SlotGridcellFallback).First;
                if (await fallback.CountAsync() > 0)
                {
                    await fallback.ClickAsync();
                    Console.WriteLine("[Step 5] Selected first available slot (no label configured).");
                    return true;
                }
            }

            Console.WriteLine("[Step 5] No available court slot found.");
            return false;
        }

        public async Task FillBookingDetailsAsync(BookingRequest request, BookingSelectors selectors, Func<Task> beforeReviewBookingsClickAsync = null)
        {
            await FillIfPresentAsync(selectors.EventNameInput, request.EventName);

            if (beforeReviewBookingsClickAsync != null)
            {
                await beforeReviewBookingsClickAsync();
            }

            await ClickIfPresentAsync(selectors.ReviewBookingsButton);

            if (!string.IsNullOrWhiteSpace(request.Player2FullName))
            {
                var group = _page.Locator(selectors.Player2QuestionGroup);
                if (await group.CountAsync() > 0)
                {
                    await group.GetByLabel(selectors.Player2InputLabel).FillAsync(request.Player2FullName);
                }
            }

            await CheckIfPresentAsync(selectors.CourtGuidelinesCheckbox);
            await CheckIfPresentAsync(selectors.NonMarkingShoesCheckbox);
            await ClickIfPresentAsync(selectors.SaveButton);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            Console.WriteLine("[Step 6] Filled booking details and saved.");
        }

        public async Task<bool> ReserveAsync(BookingSelectors selectors)
        {
            await ClickIfPresentAsync(selectors.ReserveButton);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            if (string.IsNullOrWhiteSpace(selectors.SuccessIndicator))
            {
                Console.WriteLine("[Step 7] Reserve clicked; no success selector configured.");
                return true;
            }

            var success = _page.Locator(selectors.SuccessIndicator);
            var isConfirmed = await success.IsVisibleAsync();
            Console.WriteLine($"[Step 7] Booking confirmation visible: {isConfirmed}");
            return isConfirmed;
        }

        // ------------------------------------------------------------------ //
        // Helpers
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Takes a screenshot — useful for debugging or a dry-run proof.
        /// </summary>
        public async Task TakeScreenshotAsync(string path)
        {
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
            Console.WriteLine($"Screenshot saved → {path}");
        }

        private async Task FillIfPresentAsync(string selector, string value)
        {
            if (string.IsNullOrWhiteSpace(selector) || string.IsNullOrWhiteSpace(value))
                return;

            var locator = _page.Locator(selector);
            if (await locator.CountAsync() > 0)
                await locator.FillAsync(value);
        }

        private async Task ClickIfPresentAsync(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
                return;

            var locator = _page.Locator(selector);
            if (await locator.CountAsync() > 0)
                await locator.ClickAsync();
        }

        private async Task CheckIfPresentAsync(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
                return;

            var locator = _page.Locator(selector);
            if (await locator.CountAsync() > 0 && !await locator.IsCheckedAsync())
                await locator.CheckAsync();
        }

        private static string courtType(BookingRequest r) => r.CourtType ?? "Court";
    }
}
