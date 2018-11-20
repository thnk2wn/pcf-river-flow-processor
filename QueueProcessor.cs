using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RiverFlowProcessor.RiverFlow;
using RiverFlowProcessor.USGS;

namespace RiverFlowProcessor
{
    public class QueueProcessor : IQueueProcessor
    {
        private readonly ConnectionFactory queueConnectionFactory;
        private readonly ILogger<IQueueProcessor> logger;
        private readonly IRiverFlowProcessor riverFlowProcessor;

        public QueueProcessor(
            ConnectionFactory queueConnectionFactory,
            ILogger<IQueueProcessor> logger,
            IRiverFlowProcessor riverFlowProcessor)
        {
            this.queueConnectionFactory = queueConnectionFactory;
            this.queueConnectionFactory.DispatchConsumersAsync = true;

            this.logger = logger;
            this.riverFlowProcessor = riverFlowProcessor;
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

                var args = new Dictionary<string, object>();

                const int oneHourMs = 3600000;
                args.Add("x-message-ttl", oneHourMs);

                queueChannel.QueueDeclare(
                    queue: queueName, 
                    durable: true, 
                    exclusive: false, 
                    autoDelete: false, 
                    arguments: args);

                var queueConsumer = new AsyncEventingBasicConsumer(queueChannel);
                queueConsumer.Received += async (sender, e) => 
                {
                    await ProcessMessage(queueChannel, e);
                };

                this.logger.LogInformation("Monitoring queue {queue} on {host}", queueName, queueConn.Endpoint.HostName);

                queueChannel.BasicConsume(
                    queue: queueName, 
                    autoAck: false,
                    consumer: queueConsumer);

                Console.ReadLine();
            }
        }

        private async Task ProcessMessage(IModel channel, BasicDeliverEventArgs args)
        {
            string gaugeId = null;

            try
            {
                var json = Encoding.UTF8.GetString(args.Body);
                this.logger.LogTrace("Received river flow request:{0}{1}", Environment.NewLine, json);

                var request = JsonConvert.DeserializeObject<RiverFlowRequest>(json);
                gaugeId = request.UsgsGaugeId;
                await this.riverFlowProcessor.Process(gaugeId);
                
                channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex) 
            {
                this.logger.LogError(ex, "Error processing gauge {gauge}", gaugeId ?? "unknown");
                throw;
            }
        }
    }

    public interface IQueueProcessor 
    {
        void StartListening();
    }
}