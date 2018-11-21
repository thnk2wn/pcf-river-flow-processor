using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RiverFlowProcessor.RiverFlow;

namespace RiverFlowProcessor.Queuing
{
    public class QueuePublisher : IQueuePublisher
    {
        private readonly ConnectionFactory queueConnectionFactory;
        private readonly ILogger<IQueuePublisher> logger;
        private readonly QueueProperties queueProps;
        private int publishCount;

        public QueuePublisher(
            ConnectionFactory queueConnectionFactory,
            ILogger<IQueuePublisher> logger)
        {
            this.queueConnectionFactory = queueConnectionFactory;
            this.logger = logger;
            this.queueProps = new QueueProperties();
        }

        public void PublishAll() 
        {
            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                this.logger.LogDebug(
                    "Connected to {host}. Declaring queue {queue}", 
                    queueConn.Endpoint.HostName, 
                    this.queueProps.QueueName);

                this.queueProps.DeclareQueue(queueChannel);

                queueChannel.QueuePurge(this.queueProps.QueueName);

                const string resource = "RiverFlowProcessor.usgs-sitecodes-filtered.csv";
                var assembly = typeof(QueuePublisher).GetTypeInfo().Assembly;

                using (var resourceStream = assembly.GetManifestResourceStream(resource))
                using (var streamReader = new StreamReader(resourceStream))
                using (var csv = new CsvReader(streamReader)) 
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var usgsGaugeId = csv["UsgsGaugeId"];
                        Publish(queueChannel, usgsGaugeId);
                    }
                }
            }
        }

        private void Publish(IModel queueChannel, string usgsGaugeId) 
        {
            var request = new RiverFlowRequest { UsgsGaugeId = usgsGaugeId };
            var json = JsonConvert.SerializeObject(request);

            byte[] messageBody = Encoding.UTF8.GetBytes(json);
            IBasicProperties props = queueChannel.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2;

            this.logger.LogInformation(
                "Publishing #{publishCount} - gauge {gaugeId}", 
                ++this.publishCount, 
                usgsGaugeId);

            queueChannel.BasicPublish(
                exchange: string.Empty,
                routingKey: this.queueProps.QueueName,
                basicProperties: props,
                body: messageBody);
        }
    }

    public interface IQueuePublisher
    {
        void PublishAll();
    }
}