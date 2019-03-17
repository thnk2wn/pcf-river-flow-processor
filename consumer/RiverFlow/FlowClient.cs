using System;
using System.Linq;
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
        private readonly IDiscoveryClient discoveryClient;
        private const string riverFlowApi = "river-flow-api";
        private const string RecordFlowUrl = "https://" + riverFlowApi + "/flow";

        public FlowClient(
            IDiscoveryClient discoveryClient,
            ILogger<IFlowClient> logger)
        {
            this.discoveryClient = discoveryClient;
            this.discoveryClientHandler = new DiscoveryHttpClientHandler(this.discoveryClient, logger);
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
                    "Posting to '{recordFlowUrl}' for gauge '{gaugeId}'",
                    RecordFlowUrl,
                    client.BaseAddress,
                    gaugeId);

                var instances = this.discoveryClient.GetInstances(riverFlowApi);
                var uris = string.Join(",", instances.Select(i => i.Uri.ToString()));
                this.logger.LogInformation($"{riverFlowApi} URIs: {uris}");

                // TODO: Working locally now but check server for discovery:
                // 'No connection could be made because the target machine actively refused it' - reproducable locally. Client not correctly setup?
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