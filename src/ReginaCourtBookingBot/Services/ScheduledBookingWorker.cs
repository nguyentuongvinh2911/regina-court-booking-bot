using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReginaCourtBookingBot.Config;

namespace ReginaCourtBookingBot.Services
{
    public class ScheduledBookingWorker : BackgroundService
    {
        private readonly BookingRunner _bookingRunner;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ScheduledBookingWorker> _logger;

        public ScheduledBookingWorker(
            BookingRunner bookingRunner,
            AppSettings appSettings,
            ILogger<ScheduledBookingWorker> logger)
        {
            _bookingRunner = bookingRunner;
            _appSettings = appSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!ScheduleCalculator.TryParseRunTime(_appSettings.RunAtLocalTime, out var runTime))
            {
                throw new InvalidOperationException(
                    "Service mode requires AppSettings:RunAtLocalTime to be set in HH:mm or HH:mm:ss format.");
            }

            var allowedRunDays = ScheduleCalculator.ParseAllowedRunDays(_appSettings.AllowedRunDays);
            _logger.LogInformation(
                "Scheduled booking worker started. Days={AllowedRunDays}, Time={RunTime}",
                ScheduleCalculator.DescribeDays(allowedRunDays),
                runTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = ScheduleCalculator.GetNextRun(now, runTime, allowedRunDays);
                var delay = nextRun - now;

                _logger.LogInformation("Next booking run scheduled for {NextRun:yyyy-MM-dd HH:mm:ss}", nextRun);
                await Task.Delay(delay, stoppingToken);

                try
                {
                    var success = await _bookingRunner.RunOnceAsync(waitForScheduledStart: false, stoppingToken);
                    _logger.LogInformation("Scheduled booking run completed. Success={Success}", success);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled booking run failed.");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
