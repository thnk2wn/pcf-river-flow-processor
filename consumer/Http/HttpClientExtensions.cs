using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor.Http
{
    public static class HttpClientExtensions
    {
        public static void Setup(
            this HttpClient httpClient,
            IOptions<CloudFoundryServicesOptions> serviceOptions,
            string serviceName,
            string uriCredKey = "uri")
        {
            // doc examples seem wrong - from older 1.x version maybe and not correct for 2.x. Created issue:
            // https://github.com/SteeltoeOSS/Configuration/issues/38
            var usgsSvc = serviceOptions.Value.ServicesList.Single(s => s.Name == serviceName);

            httpClient.BaseAddress = new Uri(usgsSvc.Credentials[uriCredKey].Value);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RiverFlowProcessor");
        }
    }
}