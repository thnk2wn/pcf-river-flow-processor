using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;
using Humanizer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RiverFlow.Common;
using RiverFlow.Queue;
using RiverFlowProcessor.RiverFlow;
using RiverFlowProcessor.USGS;

namespace RiverFlowProcessor.Queuing
{
    public class QueueProcessor : IQueueProcessor
    {
        private readonly ConnectionFactory queueConnectionFactory;
        private readonly ILogger<IQueueProcessor> logger;
        private readonly IRiverFlowProcessor riverFlowProcessor;
        private readonly IMetrics metrics;
        private TimerOptions queueProcessTimer;
        private CounterOptions failureCounter;

        public QueueProcessor(
            ConnectionFactory queueConnectionFactory,
            ILogger<IQueueProcessor> logger,
            IRiverFlowProcessor riverFlowProcessor,
            IMetrics metrics)
        {
            this.queueConnectionFactory = queueConnectionFactory;
            this.queueConnectionFactory.DispatchConsumersAsync = true;

            this.logger = logger;
            this.riverFlowProcessor = riverFlowProcessor;
            this.metrics = metrics;

            this.queueProcessTimer = new TimerOptions
            {
                Name = "Queue Processing Timer",
                MeasurementUnit = App.Metrics.Unit.Calls,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Minutes
            };

            this.failureCounter = new CounterOptions
            {
                Name = "Queue Processing Failures",
                MeasurementUnit = App.Metrics.Unit.Errors,
                ReportItemPercentages = true
            };
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
            using (metrics.Measure.Timer.Time(this.queueProcessTimer))
            {
                string gaugeId = null;
                var sw = Stopwatch.StartNew();

                try
                {
                    var json = Encoding.UTF8.GetString(args.Body);
                    this.logger.LogTrace("Received river flow request:{0}{1}", Environment.NewLine, json);

                    var request = JsonConvert.DeserializeObject<RiverFlowRequest>(json);
                    gaugeId = Usgs.FormatGaugeId(request.UsgsGaugeId);
                    await this.riverFlowProcessor.Process(gaugeId);

                    channel.BasicAck(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error processing gauge {gauge}", gaugeId ?? "unknown");
                    channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
                    this.metrics.Measure.Counter.Increment(this.failureCounter);
                    throw;
                }
                finally
                {
                    sw.Stop();
                    this.logger.LogInformation("Processed gauge {gauge} in {time}", gaugeId, sw.Elapsed.Humanize());
                }
            }
        }
    }

    public interface IQueueProcessor
    {
        void StartListening();
    }
}