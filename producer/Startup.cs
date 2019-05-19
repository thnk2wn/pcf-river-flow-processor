using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;
using RiverFlow.Queue;
using Steeltoe.Extensions.Configuration.ConfigServer;

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
                .AddConfigServer()
                .AddCloudFoundry()
                .AddEnvironmentVariables();
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
                .ConfigureCloudFoundryOptions(configuration)
                .Configure<QueueConfig>(configuration.GetSection("QueueConfig"));

            services.AddScoped<IQueuePublisher, QueuePublisher>();
        }
    }
}