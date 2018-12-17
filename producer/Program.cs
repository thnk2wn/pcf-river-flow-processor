using System;
using Microsoft.Extensions.DependencyInjection;

namespace RiverFlow.Producer
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
