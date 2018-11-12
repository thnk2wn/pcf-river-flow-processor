using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace river_flow_processor
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
            services.AddLogging();
            services.AddRabbitMQConnection(configuration);

            services.AddScoped<IQueueProcessor, QueueProcessor>();
        }

        private void ConfigureLogging() 
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            var loggerFactory = this.ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }
    }
}