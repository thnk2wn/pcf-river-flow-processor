using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var overrides = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("eureka:client:validateCertificates", "false"),
                new KeyValuePair<string, string>("eureka:instance:validateCertificates", "false"),
                new KeyValuePair<string, string>("eureka:instance:nonSecurePortEnabled", "true"),
                new KeyValuePair<string, string>("eureka:instance:securePortEnabled", "false")
            };

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseCloudFoundryHosting()
                .AddCloudFoundry()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddInMemoryCollection(overrides);
                });
        }
    }
}
