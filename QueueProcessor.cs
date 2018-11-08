using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace river_flow_processor
{
    public class QueueProcessor : IQueueProcessor
    {
        private readonly ConnectionFactory queueConnectionFactory;

        public QueueProcessor(ConnectionFactory queueConnectionFactory) 
        {
            this.queueConnectionFactory = queueConnectionFactory;
        }

        public void StartListening() 
        {
            const string queueName = "river-flow";

            using (var queueConn = this.queueConnectionFactory.CreateConnection())
            using (var queueChannel = queueConn.CreateModel())
            {
                queueChannel.QueueDeclare(
                    queue: queueName, 
                    durable: true, 
                    exclusive: false, 
                    autoDelete: false, 
                    arguments: null);

                var queueConsumer = new EventingBasicConsumer(queueChannel);
                queueConsumer.Received += (model, ea) =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body);
                    Console.WriteLine($"Received river flow request:{Environment.NewLine}{json}");
                };

                queueChannel.BasicConsume(
                    queue: queueName, 
                    autoAck: true,
                    consumer: queueConsumer);

                Console.ReadLine();
            }
        }
    }

    public interface IQueueProcessor 
    {
        void StartListening();
    }
}