using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace river_flow_processor
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCloudFoundry();
            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddRabbitMQConnection(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var queueConnFactory = serviceProvider.GetService<ConnectionFactory>();
            const string queueName = "river-flow";

            using (var queueConn = queueConnFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                queueChannel.QueueDeclare(
                    queue: queueName, 
                    durable: true, 
                    exclusive: false, 
                    autoDelete: false, 
                    arguments: null);

                var queueConsumer = new EventingBasicConsumer(queueChannel);
                queueConsumer.Received += (model, ea) =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body);
                    Console.WriteLine($"Received river flow request:{Environment.NewLine}{json}");
                };

                queueChannel.BasicConsume(
                    queue: queueName, 
                    autoAck: true,
                    consumer: queueConsumer);

                Console.ReadLine();
            }
        }
    }
}
