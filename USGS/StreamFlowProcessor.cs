using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor.USGS
{
    public class StreamFlowProcessor : IStreamFlowProcessor
    {
        private readonly ILogger<IStreamFlowProcessor> logger;
        private readonly string usgsIvBaseUrl;

        public StreamFlowProcessor(
            IHttpClientFactory clientFactory,
            IOptions<CloudFoundryServicesOptions> serviceOptions,
            ILogger<IStreamFlowProcessor> logger) 
        {
            this.logger = logger;

            this.logger.LogDebug(
                "Service Count: {serviceCount}: {serviceKeys}", 
                serviceOptions.Value.ServicesList.Count,
                string.Join(',', serviceOptions.Value.Services.Select(s => s.Key)));

            // doc examples seem wrong - from older 1.x version maybe and not correct for 2.x. Created issue:
            // https://github.com/SteeltoeOSS/Configuration/issues/38
            var usgsSvc = serviceOptions.Value.ServicesList.FirstOrDefault(s => s.Name == "usgs-iv");

            this.usgsIvBaseUrl = usgsSvc.Credentials["uri"].Value;
            this.logger.LogInformation("USGS IV Base Url: {usgsIvBaseUrl}", usgsIvBaseUrl);
        }

        public async Task Process(string usgsGaugeId)
        {
            var url = GetUrl(usgsGaugeId);

            // https://waterservices.usgs.gov/nwis/iv/?sites=03539600&format=json
            // var streamFlow = StreamFlow.FromJson(json);

            await Task.CompletedTask;
        }

        private string GetUrl(string usgsGaugeId)
        {
            var querystring = new Dictionary<string, string>
            {
                { "sites", usgsGaugeId },
                { "format", "json" }
            };

            var ivUrl = QueryHelpers.AddQueryString(this.usgsIvBaseUrl, querystring);
            this.logger.LogInformation("USGS IV URL: {usgsIvUrl}", ivUrl);
            return ivUrl;
        }
    }

    public interface IStreamFlowProcessor
    {
        Task Process(string usgsGaugeId);
    }
}