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
using RiverFlow.Common;
using RiverFlow.Queue;

namespace RiverFlowProducer
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

        public void Initialize()
        {
            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                InitializeQueue(queueConn, queueChannel);
            }

            this.logger.LogInformation("Exchange and queue initialized");
        }

        public void Publish(IEnumerable<string> usgsGaugeIds)
        {
            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                InitializeQueue(queueConn, queueChannel);

                foreach (var usgsGaugeId in usgsGaugeIds)
                {
                    PublishOne(queueChannel, usgsGaugeId);
                }
            }
        }

        public void Publish(int? top = null)
        {
            this.logger.LogDebug("Initializing connection to {host}", this.queueConnectionFactory.HostName);

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                InitializeQueue(queueConn, queueChannel);

                var type = typeof(QueuePublisher).GetTypeInfo();
                var resource = $"{type.Namespace}.usgs-sitecodes-filtered.csv";
                this.logger.LogInformation("Reading {resource}", resource);

                using (var resourceStream = type.Assembly.GetManifestResourceStream(resource))
                using (var streamReader = new StreamReader(resourceStream))
                using (var csv = new CsvReader(streamReader))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var read = 0;

                    while (csv.Read() && (top == null || read < top))
                    {
                        var usgsGaugeId = Usgs.FormatGaugeId(csv["UsgsGaugeId"]);
                        PublishOne(queueChannel, usgsGaugeId);
                        read++;
                    }
                }
            }
        }

        private void InitializeQueue(IConnection queueConn, IModel queueChannel)
        {
            this.logger.LogDebug(
                "Connected to {host}. Declaring queue {queue}",
                queueConn.Endpoint.HostName,
                this.queueProps.QueueName);

            SetupExchangeAndQueue(queueChannel);

            queueChannel.QueuePurge(this.queueProps.QueueName);
        }

        private void SetupExchangeAndQueue(IModel channel)
        {
            channel.ExchangeDeclare(QueueProperties.Exchange, ExchangeType.Direct);

            this.queueProps.DeclareQueue(channel);

            channel.QueueBind(
                queue: this.queueProps.QueueName,
                exchange: QueueProperties.Exchange,
                routingKey: QueueProperties.DefaultRoutingKey,
                arguments: null);
        }

        private void PublishOne(IModel queueChannel, string usgsGaugeId)
        {
            var request = new RiverFlowRequest { UsgsGaugeId = usgsGaugeId };
            var json = JsonConvert.SerializeObject(request);

            byte[] messageBody = Encoding.UTF8.GetBytes(json);
            var props = CreateMessageProps(queueChannel);

            this.logger.LogInformation(
                "Publishing #{publishCount} - gauge {gaugeId}",
                ++this.publishCount,
                usgsGaugeId);

            queueChannel.BasicPublish(
                exchange: QueueProperties.Exchange,
                routingKey: QueueProperties.DefaultRoutingKey,
                basicProperties: props,
                body: messageBody);
        }

        private IBasicProperties CreateMessageProps(IModel queueChannel)
        {
            IBasicProperties props = queueChannel.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2;
            props.Persistent = true;
            props.Expiration = QueueProperties.ExpirationMs.ToString();
            return props;
        }
    }

    public interface IQueuePublisher
    {
        void Initialize();

        void Publish(IEnumerable<string> usgsGaugeIds);

        void Publish(int? top = null);
    }
}