using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RiverFlow.Shared;
using RiverFlowProcessor.RiverFlow;
using RiverFlowProcessor.USGS;

namespace RiverFlowProcessor.Queuing
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
            var queueProps = new QueueProperties();
            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                this.logger.LogDebug(
                    "Connected to {host}. Declaring queue {queue}", 
                    queueConn.Endpoint.HostName, 
                    queueProps.QueueName);

                queueProps.DeclareQueue(queueChannel);

                var queueConsumer = new AsyncEventingBasicConsumer(queueChannel);
                queueConsumer.Received += async (sender, e) => 
                {
                    await ProcessMessage(queueChannel, e);
                };

                this.logger.LogInformation(
                    "Monitoring queue {queue} on {host}", 
                    queueProps.QueueName, 
                    queueConn.Endpoint.HostName);

                queueChannel.BasicQos(
                    prefetchCount: QueueProperties.PrefetchCount, 
                    prefetchSize: 0, 
                    global: true);

                queueChannel.BasicConsume(
                    queue: queueProps.QueueName, 
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
                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
                throw;
            }
        }
    }

    public interface IQueueProcessor 
    {
        void StartListening();
    }
}