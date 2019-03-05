using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RiverFlowProcessor.Http;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor.RiverFlow
{
    public class FlowClient : IFlowClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<IFlowClient> logger;

        public FlowClient(
            HttpClient httpClient,
            IOptions<CloudFoundryServicesOptions> serviceOptions,
            ILogger<IFlowClient> logger)
        {
            this.logger = logger;

            // TODO: use service discovery?: https://steeltoe.io/docs/steeltoe-discovery/
            httpClient.Setup(serviceOptions, serviceName: "<TODO>", uriCredKey: "uri");
            this.httpClient = httpClient;

            this.logger.LogInformation("Flow Base Url: {flowBaseUrl}", httpClient.BaseAddress);
        }

        public async Task RecordFlow(RiverFlowSnapshot snapshot)
        {
            string gaugeId = null;

            try
            {
                gaugeId = snapshot.Site.UsgsGaugeId;

                var response = await this.httpClient.PostAsync("/flow", new JsonContent(snapshot));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Error posting flow values for gauge {gauge}", gaugeId);
                throw;
            }
        }
    }

    public interface IFlowClient
    {
        Task RecordFlow(RiverFlowSnapshot snapshot);
    }
}