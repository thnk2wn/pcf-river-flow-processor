using System;

namespace RiverFlow.Queue
{
    public static class QueueConfigExtensions
    {
        public static QueueConfig Validate(this QueueConfig config)
        {
            if (config == null)
            {
                throw new NullReferenceException("queue config cannot be null");
            }

            if (string.IsNullOrEmpty(config.Exchange))
            {
                throw new InvalidOperationException("queue config default routing key must be set");
            }

            if (config.DeliveryMode != 1 && config.DeliveryMode != 2)
            {
                throw new InvalidOperationException("queue config Delivery mode should be 1 or 2.");
            }

            if (string.IsNullOrEmpty(config.Exchange))
            {
                throw new InvalidOperationException("queue config exchange must be set");
            }

            if (config.ExpirationMs <= 0)
            {
                throw new InvalidOperationException("queue config expiration time must be set");
            }

            if (string.IsNullOrEmpty(config.QueueName))
            {
                throw new InvalidOperationException("queue config name must be set");
            }

            return config;
        }
    }
}