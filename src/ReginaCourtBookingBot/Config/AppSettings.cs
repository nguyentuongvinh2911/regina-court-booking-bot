namespace ReginaCourtBookingBot.Config
{
    public class AppSettings
    {
        public string BaseUrl { get; set; } = "https://ca.apm.activecommunities.com/regina/Reserve_Options";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RunAtLocalTime { get; set; } = string.Empty;
        public string ReviewButtonClickAtLocalTime { get; set; } = "09:00:00";
        public bool StrictReviewButtonClickTime { get; set; } = true;
        public bool Headless { get; set; } = false;
        public bool DryRun { get; set; } = true;
        public int SlowMoMilliseconds { get; set; } = 50;
        public BookingSelectors Selectors { get; set; } = new();
    }

    /// <summary>
    /// CSS/text selectors for each step of the booking flow.
    /// Run:  pwsh bin/Debug/net6.0/playwright.ps1 codegen --target csharp-nunit -o RecordedBooking.cs "https://ca.apm.activecommunities.com/regina/Reserve_Options"
    /// …then copy the locators Playwright records here.
    /// </summary>
    public class BookingSelectors
    {
        public string BadmintonBookingsLink { get; set; } = "role=link[name='Badminton Bookings']";
        public string TennisBookingsLink { get; set; } = "role=link[name='Tennis Bookings']";
        public string SignInNowLink { get; set; } = "role=link[name='Sign in now']";
        public string LoginNameInput { get; set; } = "role=textbox[name='Login name Required']";
        public string PasswordInput { get; set; } = "role=textbox[name='Password Required']";
        public string DatePicker { get; set; } = "role=combobox[name='Date picker, current date']";
        public string SlotGridcellFallback { get; set; } = "role=gridcell";
        public string EventNameInput { get; set; } = "role=textbox[name='Event name']";
        public string ReviewBookingsButton { get; set; } = "[data-qa-id='quick-reservation-ok-button']";
        public string Player2QuestionGroup { get; set; } = "role=group[name=\"*What is Player 2's full name\"]";
        public string Player2InputLabel { get; set; } = "Input box";
        public string CourtGuidelinesCheckbox { get; set; } = "role=checkbox[name='I have read and agree to Court User Guidelines']";
        public string NonMarkingShoesCheckbox { get; set; } = "role=checkbox[name='I have read and agree to Non-']";
        public string SaveButton { get; set; } = "role=button[name='Save']";
        public string ReserveButton { get; set; } = "role=button[name='Total $0.00 Reserve']";
        public string SuccessIndicator { get; set; } = string.Empty;
    }
}