using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Serilog.Extensions.Logging;

namespace RiverFlowProducer
{
    public class Startup
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public Startup Configure()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCloudFoundry();
            var configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            this.ServiceProvider = services.BuildServiceProvider();

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
                .AddOptions()
                .ConfigureCloudFoundryOptions(configuration);

            services.AddScoped<IQueuePublisher, QueuePublisher>();
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