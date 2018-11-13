using System;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace river_flow_processor
{
    public class QueueProcessor : IQueueProcessor
    {
        private readonly ConnectionFactory queueConnectionFactory;
        private readonly ILogger<IQueueProcessor> logger;

        public QueueProcessor(
            ConnectionFactory queueConnectionFactory,
            ILogger<IQueueProcessor> logger)
        {
            this.queueConnectionFactory = queueConnectionFactory;
            this.logger = logger;
        }

        public void StartListening() 
        {
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            if (string.IsNullOrEmpty(queueName))
                throw new InvalidOperationException("QUEUE_NAME must be set in environment to listen");

            this.logger.LogDebug("Initializing connection to {0}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                this.logger.LogDebug("Connected to {0}. Declaring queue {1}", queueConn.Endpoint.HostName, queueName);

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
                    this.logger.LogInformation("Received river flow request:{0}{1}", Environment.NewLine, json);
                };

                this.logger.LogInformation("Monitoring queue {0} on {1}", queueName, queueConn.Endpoint.HostName);

                queueChannel.BasicConsume(
                    queue: queueName, 
                    autoAck: true,
                    consumer: queueConsumer);

                Console.ReadLine();
            }
        }
    }

    public interface IQueueProcessor 
    {
        void StartListening();
    }
}