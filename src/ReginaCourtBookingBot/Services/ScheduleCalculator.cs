using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ReginaCourtBookingBot.Services
{
    public static class ScheduleCalculator
    {
        public static bool TryParseRunTime(string runAtLocalTime, out TimeSpan scheduledTime)
        {
            if (string.IsNullOrWhiteSpace(runAtLocalTime))
            {
                scheduledTime = default;
                return false;
            }

            if (!TimeSpan.TryParseExact(
                runAtLocalTime,
                new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss", @"H\:mm\:ss" },
                CultureInfo.InvariantCulture,
                out scheduledTime))
            {
                throw new InvalidOperationException("AppSettings:RunAtLocalTime must be in HH:mm or HH:mm:ss format.");
            }

            return true;
        }

        public static IReadOnlySet<DayOfWeek> ParseAllowedRunDays(string allowedRunDays)
        {
            if (string.IsNullOrWhiteSpace(allowedRunDays))
            {
                return new HashSet<DayOfWeek>();
            }

            var days = new HashSet<DayOfWeek>();
            var values = allowedRunDays
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var value in values)
            {
                if (!Enum.TryParse<DayOfWeek>(value, ignoreCase: true, out var day))
                {
                    throw new InvalidOperationException(
                        $"AppSettings:AllowedRunDays contains invalid day '{value}'. Use names like Monday,Wednesday,Friday.");
                }

                days.Add(day);
            }

            return days;
        }

        public static DateTime GetNextRun(DateTime now, TimeSpan runTime, IReadOnlySet<DayOfWeek> allowedRunDays)
        {
            for (var dayOffset = 0; dayOffset < 14; dayOffset++)
            {
                var candidateDate = now.Date.AddDays(dayOffset);
                if (allowedRunDays.Count > 0 && !allowedRunDays.Contains(candidateDate.DayOfWeek))
                {
                    continue;
                }

                var candidateDateTime = candidateDate.Add(runTime);
                if (candidateDateTime > now)
                {
                    return candidateDateTime;
                }
            }

            throw new InvalidOperationException("Unable to calculate the next scheduled booking run.");
        }

        public static string DescribeDays(IReadOnlySet<DayOfWeek> allowedRunDays)
        {
            return allowedRunDays.Count == 0
                ? "Every day"
                : string.Join(", ", allowedRunDays.OrderBy(day => day));
        }
    }
}
