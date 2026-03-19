using System.Collections.Generic;

namespace ReginaCourtBookingBot.Models
{
    public class BookingRequest
    {
        public System.DateTime Date { get; set; }

        /// <summary>"Badminton" or "Tennis" — matches the landing-page tile text.</summary>
        public string CourtType { get; set; } = "Badminton";

        /// <summary>Accessible name of the first-choice slot to click.</summary>
        public string SlotLabel { get; set; } = string.Empty;

        /// <summary>
        /// Ordered list of fallback slot labels to try if SlotLabel is not available.
        /// Each entry is tried in sequence until one is found.
        /// </summary>
        public List<string> SlotFallbacks { get; set; } = new();

        /// <summary>Value entered into the Event name field.</summary>
        public string EventName { get; set; } = "1";

        /// <summary>Value entered for the second player prompt.</summary>
        public string Player2FullName { get; set; } = string.Empty;
    }
}
