using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverFlowProcessor.RiverFlow;
using RiverFlowProcessor.USGS;
using Serilog;
using Serilog.Events;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor
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

            this.ConfigureLogging();
            
            return this;
        }

        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration) 
        {
            services.AddLogging()
                .AddRabbitMQConnection(configuration)
                .AddHttpClient()
                .AddOptions()
                .ConfigureCloudFoundryOptions(configuration);

            services.AddHttpClient<IUsgsIvClient, UsgsIvClient>();

            services.AddScoped<IQueueProcessor, QueueProcessor>();
            services.AddScoped<IRiverFlowProcessor, RiverFlowProcessor.RiverFlow.RiverFlowProcessor>();
        }

        private void ConfigureLogging()
        {
            var logLevel = GetLogLevel();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .WriteTo.Console(
                    outputTemplate: "{SourceContext}: [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Console.WriteLine("Log level: {0}, enabled: {1}", logLevel, Log.Logger.IsEnabled(logLevel));

            var loggerFactory = this.ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }

        private LogEventLevel GetLogLevel() 
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