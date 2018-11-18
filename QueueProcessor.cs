using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RiverFlowProcessor.USGS;

namespace RiverFlowProcessor
{
    public class QueueProcessor : IQueueProcessor
    {
        private readonly ConnectionFactory queueConnectionFactory;
        private readonly ILogger<IQueueProcessor> logger;
        private readonly IStreamFlowProcessor streamFlowProcessor;

        public QueueProcessor(
            ConnectionFactory queueConnectionFactory,
            ILogger<IQueueProcessor> logger,
            IStreamFlowProcessor streamFlowProcessor)
        {
            this.queueConnectionFactory = queueConnectionFactory;
            this.queueConnectionFactory.DispatchConsumersAsync = true;

            this.logger = logger;
            this.streamFlowProcessor = streamFlowProcessor;
        }

        public void StartListening() 
        {
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            if (string.IsNullOrEmpty(queueName))
                throw new InvalidOperationException("QUEUE_NAME must be set in environment to listen");

            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                this.logger.LogDebug("Connected to {host}. Declaring queue {queue}", queueConn.Endpoint.HostName, queueName);

                queueChannel.QueueDeclare(
                    queue: queueName, 
                    durable: true, 
                    exclusive: false, 
                    autoDelete: false, 
                    arguments: null);

                var queueConsumer = new AsyncEventingBasicConsumer(queueChannel);
                queueConsumer.Received += AfterMessageReceived;

                this.logger.LogInformation("Monitoring queue {queue} on {host}", queueName, queueConn.Endpoint.HostName);

                queueChannel.BasicConsume(
                    queue: queueName, 
                    autoAck: true,
                    consumer: queueConsumer);

                Console.ReadLine();
            }
        }

        private async Task AfterMessageReceived(object sender, BasicDeliverEventArgs args)
        {
            var json = Encoding.UTF8.GetString(args.Body);
            this.logger.LogInformation("Received river flow request:{0}{1}", Environment.NewLine, json);

            var request = JsonConvert.DeserializeObject<RiverFlowRequest>(json);
            await this.streamFlowProcessor.Process(request.UsgsGaugeId);
        }
    }

    public interface IQueueProcessor 
    {
        void StartListening();
    }
}