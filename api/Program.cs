using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
                .AddCloudFoundry();
        }
    }
}
