using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using RiverFlowProcessor.Queuing;

namespace RiverFlowProcessor
{
    class Program
    {
        private static Timer timer;

        private static IMetrics metrics;

        static void Main(string[] args)
        {
            Console.WriteLine("App starting up. Args: {0}", string.Join(",", args));

            var serviceProvider = new Startup().Configure().ServiceProvider;
            if (serviceProvider == null) throw new NullReferenceException("Service provider not set");

            metrics = serviceProvider.GetService<IMetrics>();

            timer = new Timer(
                callback: new TimerCallback(TimerTaskAsync),
                state: null,
                dueTime: 30000,
                period: 30000);

            var queueProcessor = serviceProvider.GetService<IQueueProcessor>();
            queueProcessor.StartListening();
        }

        private static void Consume(ServiceProvider serviceProvider) 
        {
            Console.WriteLine("Consume queue");

            var queueProcessor = serviceProvider.GetService<IQueueProcessor>();
            queueProcessor.StartListening();
        }

        private static async void TimerTaskAsync(object timerState) 
        {
            var metricsRoot = (IMetricsRoot)metrics;
            await Task.WhenAll(metricsRoot.ReportRunner.RunAllAsync());
        }
    }
}
