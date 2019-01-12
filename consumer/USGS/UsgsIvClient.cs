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
    public class UsgsIvClient : IUsgsIvClient
    {
        private readonly HttpClient client;
        private readonly ILogger<IUsgsIvClient> logger;

        public UsgsIvClient(
            HttpClient httpClient,
            IOptions<CloudFoundryServicesOptions> serviceOptions,
            ILogger<IUsgsIvClient> logger)
        {
            this.logger = logger;

            // doc examples seem wrong - from older 1.x version maybe and not correct for 2.x. Created issue:
            // https://github.com/SteeltoeOSS/Configuration/issues/38
            var usgsSvc = serviceOptions.Value.ServicesList.Single(s => s.Name == "usgs-iv");

            httpClient.BaseAddress = new Uri(usgsSvc.Credentials["uri"].Value);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RiverFlowProcessor");
            this.client = httpClient;

            this.logger.LogInformation("USGS IV Base Url: {usgsIvBaseUrl}", httpClient.BaseAddress);
        }

        public async Task<StreamFlow> GetStreamFlow(params string[] sites)
        {
            var relativeUrl = GetRelativeUrl(sites);
            var usgsIvUrl = $"{this.client.BaseAddress}{relativeUrl}";

            // i.e. https://waterservices.usgs.gov/nwis/iv/?sites=03539600&format=json&variable=00060,00065,00010
            this.logger.LogInformation("Getting stream flow using: {usgsIvUrl}", usgsIvUrl);

            var json = await this.client.GetStringAsync(relativeUrl);
            this.logger.LogTrace(json);

            this.logger.LogTrace(
                "Parsing {length} bytes of json for sites {sites}",
                json.Length,
                string.Join(',', sites));
            var streamFlow = StreamFlow.FromJson(json);

            this.logger.LogInformation(
                "{length} bytes of JSON returned and parsed for {usgsIvUrl}",
                json.Length,
                usgsIvUrl);

            return streamFlow;
        }

        private string GetRelativeUrl(params string[] sites)
        {
            var querystring = new Dictionary<string, string>
            {
                { "sites", string.Join(',', sites) },
                {
                    "variable",
                    string.Join(
                        ',',
                        new[] {
                            UsgsVariables.DischargeCFS,
                            UsgsVariables.GaugeHeightFeet,
                            UsgsVariables.WaterTempCelsius} )
                        },
                { "format", "json" }
            };

            var ivUrl = QueryHelpers.AddQueryString("", querystring);
            return ivUrl;
        }
    }

    public interface IUsgsIvClient
    {
        Task<StreamFlow> GetStreamFlow(params string[] sites);
    }
}