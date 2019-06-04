using System;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace RiverFlowProducer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"Producer starting with args: {string.Join(", ", args)}");
            int returnCode = 0;

            using (var startup = new Startup().Configure())
            {
                if (startup.ServiceProvider == null)
                {
                    throw new NullReferenceException("Service provider not set");
                }

                var publisher = startup.ServiceProvider.GetService<IQueuePublisher>();
                returnCode = RunPublish(publisher, args);
            }

            // HACK: despite disposal, logs not getting flushed at full speed
            Thread.Sleep(5000);
            return returnCode;
        }

        private static int RunPublish(IQueuePublisher publisher, string[] args)
        {
            using (var app = new CommandLineApplication())
            {
                app.ThrowOnUnexpectedArgument = false;

                app.HelpOption("-h|--help");
                app.ExtendedHelpText = "Publishes queue message(s) and/or initializes exchange and queue.";

                var optionGaugeIds = app.Option(
                    "-g|--gauges <usgsGaugeIds>",
                    "List of USGS gauge ids to publish",
                    CommandOptionType.MultipleValue);

                var optionAll = app.Option(
                    "-a|--all",
                    "Publishes messages to refresh all USGS gauges",
                    CommandOptionType.NoValue
                );

                var optionTop = app.Option<int>(
                    "-t|--top",
                    "Publishes messages to refresh top N USGS gauges (i.e. limits for bulk testing)",
                    CommandOptionType.SingleValue
                ).Accepts(o => o.Range(1, 1000));

                app.Option("--server.urls <urls>", "PCF server urls", CommandOptionType.SingleValue);

                app.OnExecute(() =>
                {
                    publisher.Initialize();

                    if (optionGaugeIds.HasValue())
                    {
                        publisher.Publish(optionGaugeIds.Values);
                    }
                    else if (optionTop.HasValue())
                    {
                        publisher.Publish(top: optionTop.ParsedValue);
                    }
                    else if (optionAll.HasValue())
                    {
                        publisher.Publish(top: null);
                    }
                    else
                    {
                        Console.WriteLine("Producer initialized, no queue messages published");
                    }
                });

                var returnCode = app.Execute(args);

                if (returnCode > 0)
                {
                    // Show help on validation issues.
                    app.ShowHelp();
                }
                else if (returnCode < 0)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Program terminated with error; see logs for details.");
                    Console.ForegroundColor = color;
                }
                else
                {
                    Console.WriteLine("Producer processing complete.");
                }

                return returnCode;
            }
        }
    }
}
