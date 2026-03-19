using System;
using ReginaCourtBookingBot.Services;
using Xunit;

namespace ReginaCourtBookingBot.Tests
{
    public class ScheduleCalculatorTests
    {
        [Fact]
        public void ParseAllowedRunDays_ReturnsEmptySet_WhenValueIsBlank()
        {
            var result = ScheduleCalculator.ParseAllowedRunDays(string.Empty);

            Assert.Empty(result);
        }

        [Fact]
        public void ParseAllowedRunDays_Throws_WhenValueContainsInvalidDay()
        {
            var action = () => ScheduleCalculator.ParseAllowedRunDays("Monday,Funday");

            var exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Contains("Funday", exception.Message);
        }

        [Fact]
        public void GetNextRun_ReturnsSameDay_WhenFutureTimeIsAllowed()
        {
            var now = new DateTime(2026, 3, 19, 8, 30, 0);
            var allowedDays = ScheduleCalculator.ParseAllowedRunDays("Thursday");

            var result = ScheduleCalculator.GetNextRun(now, new TimeSpan(9, 0, 0), allowedDays);

            Assert.Equal(new DateTime(2026, 3, 19, 9, 0, 0), result);
        }

        [Fact]
        public void GetNextRun_SkipsToNextAllowedDay_WhenTodayIsNotAllowed()
        {
            var now = new DateTime(2026, 3, 19, 8, 30, 0);
            var allowedDays = ScheduleCalculator.ParseAllowedRunDays("Friday");

            var result = ScheduleCalculator.GetNextRun(now, new TimeSpan(9, 0, 0), allowedDays);

            Assert.Equal(new DateTime(2026, 3, 20, 9, 0, 0), result);
        }

        [Fact]
        public void GetNextRun_SkipsPastRunTimeToNextMatchingDay()
        {
            var now = new DateTime(2026, 3, 19, 10, 0, 0);
            var allowedDays = ScheduleCalculator.ParseAllowedRunDays("Thursday");

            var result = ScheduleCalculator.GetNextRun(now, new TimeSpan(9, 0, 0), allowedDays);

            Assert.Equal(new DateTime(2026, 3, 26, 9, 0, 0), result);
        }
    }
}
