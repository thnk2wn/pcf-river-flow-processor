using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

[assembly: AssemblyUsage(
    " ",
    "river-flow-processor -m mode", 
    " ",
    "Consumes river flow requests from a queue by default or with mode c.",
    " ")
]

namespace RiverFlowProcessor
{
    class CommandLineOptions
    {
        [Option(
            'm', 
            "mode", 
            Default = "c",
            HelpText = "App mode - c for consumer, p for producer."
        )]
        public string Mode { get; set; }

        [Usage(ApplicationAlias = "river-flow-processor")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "\tProduce river flow requests, publishing all rivers to queue", 
                        new CommandLineOptions { Mode = "p" })
                };
            }
        }
    }
}