using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            
            return this;
        }

        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration) 
        {
            services.AddRabbitMQConnection(configuration);

            services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            services.AddScoped<IQueueProcessor, QueueProcessor>();
        }
    }
}