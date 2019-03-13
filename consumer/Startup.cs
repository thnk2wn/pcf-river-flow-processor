using System;
using App.Metrics;
using App.Metrics.Filtering;
using App.Metrics.Formatters.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverFlowProcessor.Queuing;
using RiverFlowProcessor.RiverFlow;
using RiverFlowProcessor.USGS;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor
{
    public class Startup
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public IDiscoveryClient DiscoveryClient { get; private set;}

        public Startup Configure()
        {
            // TODO: Can't use .UseDiscoveryClient(). See: SteeltoeSamples\Discovery\src\Console\Register\Program.cs
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCloudFoundry();
            var configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            this.ServiceProvider = services.BuildServiceProvider();

            this.DiscoveryClient = this.UseDiscoveryClient();

            return this;
        }

        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services
                .AddLogging(builder =>
                {
                    var logger = CreateLogger();
                    builder.AddSerilog(logger, dispose: true);
                    Log.Logger = logger;
                })
                .AddRabbitMQConnection(configuration)
                .AddHttpClient()
                .AddOptions()
                .AddDiscoveryClient(configuration)
                .ConfigureCloudFoundryOptions(configuration);

            services.AddHttpClient<IUsgsIvClient, UsgsIvClient>();

            services.AddScoped<IFlowClient, FlowClient>();

            services.AddScoped<IQueueProcessor, QueueProcessor>();
            services.AddScoped<IRiverFlowProcessor, RiverFlowProcessor.RiverFlow.RiverFlowProcessor>();

            services.AddSingleton<IMetrics>(CreateMetrics());
        }

        private IDiscoveryClient UseDiscoveryClient()
        {
            // this is what Steeltoe's IApplicationBuilder.UseDiscoveryClient does.
            // since not an asp.net app we're not using here but we are using DI/IOC
            var service = this.ServiceProvider.GetRequiredService<IDiscoveryClient>();

            // make sure that the lifcycle object is created (this is specific to asp.net)
            // var lifecycle = this.ServiceProvider.GetService<IDiscoveryLifecycle>();

            return service;
        }

        private static IMetrics CreateMetrics()
        {
            var filter = new MetricsFilter().WhereType(MetricType.Timer);
            var metrics = new MetricsBuilder()
                .OutputMetrics.Using<TimerMetricsFormatter>()
                .Report.ToConsole(
                    options => {
                        options.FlushInterval = TimeSpan.FromSeconds(30);
                        options.Filter = filter;
                        options.MetricsOutputFormatter = new TimerMetricsFormatter();
                    })
                .Build();
            return metrics;
        }

        private static Serilog.ILogger CreateLogger()
        {
            var logLevel = GetLogLevel();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    // Add {SourceContext} for logger class name (with namespace)
                    outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            return logger;
        }

        private static LogEventLevel GetLogLevel()
        {
            var rawLogLevel = Environment.GetEnvironmentVariable("LOG_LEVEL");

            if (string.IsNullOrEmpty(rawLogLevel))
            {
                rawLogLevel = "Information";
                Console.WriteLine($"LOG_LEVEL not set in environment, defaulting to {rawLogLevel}");
            }

            if (!Enum.TryParse(rawLogLevel, true, out LogEventLevel logLevel))
            {
                var validLevels = string.Join(',', Enum.GetNames(typeof(LogEventLevel)));
                throw new InvalidOperationException(
                    $"Invalid log level '{rawLogLevel}'. Expected one of: {validLevels}");
            }

            Console.WriteLine($"Log level is {logLevel}");

            return logLevel;
        }
    }
}