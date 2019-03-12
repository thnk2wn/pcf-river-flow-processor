using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RiverFlowProcessor.Http;
using Steeltoe.Common.Discovery;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace RiverFlowProcessor.RiverFlow
{
    public class FlowClient : IFlowClient
    {
        private readonly ILogger<IFlowClient> logger;
        private readonly DiscoveryHttpClientHandler discoveryClientHandler;
        private const string RecordFlowUrl = "http://river-flow-api/flow";

        public FlowClient(
            IDiscoveryClient discoveryClient,
            ILogger<IFlowClient> logger)
        {
            this.discoveryClientHandler = new DiscoveryHttpClientHandler(discoveryClient, logger);
            this.logger = logger;
        }

        public async Task RecordFlow(RiverFlowSnapshot snapshot)
        {
            string gaugeId = null;

            try
            {
                gaugeId = snapshot.Site.UsgsGaugeId;
                var client = CreateHttpClient();
                this.logger.LogInformation(
                    "Posting to '{recordFlowUrl}' with base address '{baseAddress}' for gauge '{gaugeId}'",
                    RecordFlowUrl,
                    client.BaseAddress,
                    gaugeId);
                var response = await client.PostAsync(RecordFlowUrl, new JsonContent(snapshot));

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Error posting flow values for gauge {gauge}", gaugeId);
                throw;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient(this.discoveryClientHandler, disposeHandler: false);
            return client;
        }
    }

    public interface IFlowClient
    {
        Task RecordFlow(RiverFlowSnapshot snapshot);
    }
}