using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Configuration.ConfigServer;
using static Steeltoe.Extensions.Configuration.CloudFoundry.CloudFoundryHostBuilderExtensions;

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
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            // Gets around ambiguity between CloudFoundry and ConfigServer in Steeltoe.Extensions.Configuration:
            CloudFoundryHostBuilderExtensions.AddCloudFoundry(builder);

            builder
                // Add VCAP_* configuration data
                .AddCloudFoundry()

                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile(
                            $"appsettings.{env.EnvironmentName}.json",
                            optional: false,
                            reloadOnChange: true);

                    config.AddConfigServer(env.EnvironmentName);

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                });
            return builder;
        }
    }
}
