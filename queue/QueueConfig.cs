namespace RiverFlow.Queue
{
    public class QueueConfig
    {
        public string DefaultRoutingKey { get; set; }

        public byte DeliveryMode { get; set; }

        public bool Durable { get; set; }

        public string Exchange { get; set; }

        public int ExpirationMs { get; set; }

        public bool FailureRequeue { get; set; }

        public bool MultipleAck { get; set; }

        public ushort PrefetchCount { get; set; }

        public string QueueName { get; set; }
    }
}