using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;

namespace RiverFlowProducer
{
    public class Startup
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public Startup Configure()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{envName}.json", optional: false)
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
                .AddLogging(loggingBuilder => {
                    loggingBuilder.AddConsole();
                })
                .AddRabbitMQConnection(configuration)
                .AddOptions()
                .ConfigureCloudFoundryOptions(configuration);

            services.AddScoped<IQueuePublisher, QueuePublisher>();
        }
    }
}