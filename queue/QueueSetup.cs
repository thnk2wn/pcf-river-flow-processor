using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RiverFlow.Queue
{
    public class QueueSetup
    {
        private readonly QueueConfig config;

        public QueueSetup(QueueConfig config)
        {
            this.config = config;
        }

        public void DeclareQueue(IModel channel)
        {
            var args = new Dictionary<string, object>();
            args.Add("x-message-ttl", this.config.ExpirationMs);

            channel.QueueDeclare(
                queue: this.config.QueueName,

                // Durable incurrs more disk I/O. We'll likely get flow data every hour (same messages).
                // Not critical if rabbitmq restart leads to some messages not getting processed.
                durable: this.config.Durable,

                exclusive: false,
                autoDelete: false,
                arguments: args);
        }
    }
}