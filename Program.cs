using System;
using Microsoft.Extensions.DependencyInjection;

namespace RiverFlowProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new Startup().Configure().ServiceProvider;
            if (serviceProvider == null) throw new NullReferenceException("Service provider not set");

            var queueProcessor = serviceProvider.GetService<IQueueProcessor>();
            queueProcessor.StartListening();
        }
    }
}
