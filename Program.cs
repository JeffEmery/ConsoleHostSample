using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleHostSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Entering Main!");

            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<WorkerOptions>(context.Configuration.GetSection(WorkerOptions.Settings));
                    //services.AddHostedService<LoopingHostedService>();
                    services.AddHostedService<LoopingBackgroundService>();
                });
        }
    }
    public class LoopingBackgroundService : BackgroundService
    {
        private readonly ILogger<LoopingBackgroundService> logger;
        private readonly WorkerOptions options;

        public LoopingBackgroundService(ILogger<LoopingBackgroundService> logger, IOptions<WorkerOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => logger.LogDebug("Worker loop received cancel"));

            logger.LogInformation("Entering worker loop");

            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(options.Delay, stoppingToken);
                    logger.LogInformation($"Doing work... waiting {options.Delay}");
                }
            }, stoppingToken).ConfigureAwait(false);

            logger.LogInformation("Exiting worker loop");

            return Task.CompletedTask;
        }
    }

    public class LoopingHostedService : IHostedService
    {
        private readonly IConfiguration config;
        private readonly WorkerOptions options;
        private readonly ILogger logger;
        private readonly IHostEnvironment hostEnvironment;

        public LoopingHostedService(IConfiguration config, IOptions<WorkerOptions> options, ILogger<LoopingHostedService> logger, IHostEnvironment hostEnvironment)
        {
            this.config = config;
            this.options = options.Value;
            this.logger = logger;
            this.hostEnvironment = hostEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Hello from StartAsync");
            var loopTask = WorkerLoop(cancellationToken);
            logger.LogInformation("Could do more things while loop runs...");
            return loopTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Goodbye from StopAsync");
            return Task.CompletedTask;
        }

        private Task WorkerLoop(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => logger.LogDebug("Worker loop received cancel"));

            logger.LogInformation("Entering worker loop");

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(options.Delay);
                    logger.LogInformation($"Doing work... waiting {options.Delay}");
                }
            }, cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Exiting worker loop");

            return Task.CompletedTask;
        }
    }

    public class WorkerOptions
    {
        public const string Settings = "WorkerOptions";
        public string Message { get; set; }
        public int Delay { get; set; }
    }

}
