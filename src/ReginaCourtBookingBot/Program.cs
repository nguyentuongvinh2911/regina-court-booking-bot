using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReginaCourtBookingBot.Config;
using ReginaCourtBookingBot.Services;

namespace ReginaCourtBookingBot
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var runMode = ParseRunMode(args);
            if (runMode == RunMode.Help)
            {
                PrintUsage();
                return;
            }

            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory
            });

            builder.Configuration.Sources.Clear();
            builder.Configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("secrets.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>(optional: true)
                .AddEnvironmentVariables(prefix: "BOOKINGBOT_");

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
            builder.Services.AddSingleton<BookingRunner>();

            if (runMode == RunMode.Service)
            {
                builder.Services.AddWindowsService(options =>
                {
                    options.ServiceName = "ReginaCourtBookingBot";
                });
                builder.Services.AddHostedService<ScheduledBookingWorker>();
            }

            using var host = builder.Build();

            if (runMode == RunMode.Once)
            {
                var runner = host.Services.GetRequiredService<BookingRunner>();
                var success = await runner.RunOnceAsync(waitForScheduledStart: true);
                Environment.ExitCode = success ? 0 : 1;
                return;
            }

            await host.RunAsync();
        }

        private static RunMode ParseRunMode(string[] args)
        {
            if (args.Any(arg => string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase)
                || string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase)
                || string.Equals(arg, "/?", StringComparison.OrdinalIgnoreCase)))
            {
                return RunMode.Help;
            }

            if (args.Any(arg => string.Equals(arg, "--service", StringComparison.OrdinalIgnoreCase)
                || string.Equals(arg, "--schedule", StringComparison.OrdinalIgnoreCase)))
            {
                return RunMode.Service;
            }

            return RunMode.Once;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Regina Court Booking Bot");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  ReginaCourtBookingBot.exe [--once]");
            Console.WriteLine("  ReginaCourtBookingBot.exe --service");
            Console.WriteLine();
            Console.WriteLine("Modes:");
            Console.WriteLine("  --once     Run the booking flow one time (default).");
            Console.WriteLine("  --service  Run as a long-lived scheduled worker using AppSettings.AllowedRunDays and AppSettings.RunAtLocalTime.");
        }

        private enum RunMode
        {
            Once,
            Service,
            Help
        }
    }
}