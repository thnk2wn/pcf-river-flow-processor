using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace RiverFlowProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new Startup().Configure().ServiceProvider;
            if (serviceProvider == null) throw new NullReferenceException("Service provider not set");

            var publisher = serviceProvider.GetService<IQueuePublisher>();

            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            app.ExtendedHelpText = "Publishes queue messages and/or initializes exchange and queue. " + Environment.NewLine +
                "Pass --init to initialize exchange and queue otherwise all messsages published.";

            var optionInit = app.Option(
                template: "--init",
                description: $"If set exchange is created and queue or purged if already there.",
                optionType: CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                if (optionInit.HasValue())
                {
                    publisher.Initialize();
                }
                else
                {
                    publisher.PublishAll();
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
        }
    }
}
