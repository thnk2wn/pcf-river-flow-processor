namespace RiverFlow.Queue
{
    public class QueueConfig
    {
        public int ExpirationMs { get; set; }

        public string Exchange { get; set; }

        public string DefaultRoutingKey { get; set; }

        public ushort PrefetchCount { get; set; }

        public string QueueName { get; set; }

        public bool FailureRequeue { get; set; }

        public bool MultipleAck { get; set; }
    }
}