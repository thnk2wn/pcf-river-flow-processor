using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;
using RiverFlow.Queue;
using Steeltoe.Extensions.Configuration.ConfigServer;
using RiverFlow.Common;

namespace RiverFlowProducer
{
    public class Startup : DisposableObject
    {
        public Startup()
        {
        }

        public ServiceProvider ServiceProvider { get; private set; }

        private ServiceCollection ServiceCollection { get; set; }

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

            this.ServiceCollection = new ServiceCollection();
            ConfigureServices(this.ServiceCollection, configuration);
            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();

            return this;
        }

        protected override void DisposeManagedResources()
        {
            Console.WriteLine("Disposing service collection");
            // required to get logging to flush otherwise those buffered may not get logged when app exits
            this.ServiceProvider?.Dispose();
            (this.ServiceCollection as IDisposable)?.Dispose();
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