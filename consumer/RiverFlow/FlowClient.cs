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
        private Uri apiBaseUri;

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
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            string gaugeId = null;

            try
            {
                gaugeId = snapshot.Site.UsgsGaugeId;
                var client = CreateHttpClient();
                var uri = this.apiBaseUri ?? (apiBaseUri = this.GetApiBaseUri());
                this.logger.LogInformation(
                    "Posting to '{recordFlowUrl}' ({uri}) for gauge '{gaugeId}'",
                    RecordFlowUrl,
                    uri,
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

        private Uri GetApiBaseUri()
        {
            this.logger.LogDebug("Using discovery client to get instances of {api}", riverFlowApi);
            var instances = this.discoveryClient.GetInstances(riverFlowApi);

            if (instances?.Count != 1)
            {
                throw new InvalidOperationException($"{instances?.Count} instances found for {riverFlowApi}. Expected 1");
            }

            var uri = instances[0].Uri;
            return uri;
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