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
        private static Dictionary<string, Action<ServiceProvider>> modeMap 
            = new Dictionary<string, Action<ServiceProvider>> 
            { 
                { "c", Consume },
                { "p", Produce }
            };
        
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

            try 
            {
                var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);

                parseResult
                    .WithParsed(options => 
                    {
                        var mode = options.Mode.Substring(0, 1).ToLowerInvariant();

                        if (modeMap.ContainsKey(mode)) 
                        {
                            modeMap[mode](serviceProvider);
                        }
                        else
                        {
                            var helpText = HelpText.AutoBuild(
                                parseResult, 
                                (err) => { return err; }, 
                                (example) => { return example; });

                            Console.Error.WriteLine(helpText);
                            Console.Error.WriteLine("Invalid mode argument; refer to usage.");
                        }
                    })
                    .WithNotParsed(errors => 
                    {
                        //Console.Error.WriteLine("Error parsing command line; refer to usage.");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: issue parsing command line: {0}.", ex.Message);
                Consume(serviceProvider);
            }
        }

        private static void Consume(ServiceProvider serviceProvider) 
        {
            Console.WriteLine("Consume queue");

            var queueProcessor = serviceProvider.GetService<IQueueProcessor>();
            queueProcessor.StartListening();
        }

        private static void Produce(ServiceProvider serviceProvider) 
        {
            Console.WriteLine("Produce queue");

            var publisher = serviceProvider.GetService<IQueuePublisher>();
            publisher.PublishAll();
        }

        private static async void TimerTaskAsync(object timerState) 
        {
            var metricsRoot = (IMetricsRoot)metrics;
            await Task.WhenAll(metricsRoot.ReportRunner.RunAllAsync());
        }
    }
}
