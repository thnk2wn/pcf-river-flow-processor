using System;
using System.Collections.Generic;
using System.Linq;
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
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseCloudFoundryHosting()
                .AddCloudFoundry()
                .ConfigureAppConfiguration(config =>
                {
                    var appSettings = GetCustomAppSettings();

                    if (appSettings != null)
                    {
                        var overrides = string.Join(", ", appSettings.CloudFoundryOverrides.Select(x => $"{x.Key}:{x.Value}"));
                        Console.WriteLine($"CF overrides: {overrides}");
                        // Currently have a need to override some service registry service binding config
                        // to target HTTP as target PCF instance has HTTPS set but cert issues accessing
                        config.AddInMemoryCollection(appSettings.CloudFoundryOverrides);
                    }
                });
        }

        private static AppSettings GetCustomAppSettings()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var tempOverrideConfig = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{envName}.json", optional: true)
                .Build();

            var customAppSettings = new AppSettings();
            tempOverrideConfig.Bind("CustomAppSettings", customAppSettings);

            // can't use ":" in file config provider keys as specical meaning for objects/sections
            customAppSettings.CloudFoundryOverrides =
                customAppSettings
                .CloudFoundryOverrides
                .ToDictionary(k => k.Key.Replace("_", ":"), e => e.Value);

            return customAppSettings;
        }
    }
}
