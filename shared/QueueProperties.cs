using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RiverFlow.Shared
{
    public class QueueProperties
    {
        public const int ExpirationMs = 3600000;

        public const string Exchange = "RiverFlowExchange";

        public const string DefaultRoutingKey = "FlowKey";

        public const int PrefetchCount = 1;

        public string QueueName { get; }

        public QueueProperties()
        {
            this.QueueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            if (string.IsNullOrEmpty(this.QueueName))
                throw new InvalidOperationException("QUEUE_NAME must be set in environment");
        }

        public void DeclareQueue(IModel channel) 
        {
            var args = new Dictionary<string, object>();
            args.Add("x-message-ttl", ExpirationMs);

            channel.QueueDeclare(
                queue: this.QueueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: args);
        }
    }
}