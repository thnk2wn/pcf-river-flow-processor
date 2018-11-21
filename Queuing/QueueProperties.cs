using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RiverFlowProcessor.Queuing
{
    public class QueueProperties
    {
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

            const int oneHourMs = 3600000;
            args.Add("x-message-ttl", oneHourMs);

            channel.QueueDeclare(
                queue: this.QueueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: args);
        }
    }
}