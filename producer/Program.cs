using System;
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
            publisher.PublishAll();
        }
    }
}
