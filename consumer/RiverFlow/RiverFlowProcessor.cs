using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Timer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RiverFlow.Shared;
using RiverFlowProcessor.USGS;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using TimeZone = RiverFlowProcessor.USGS.TimeZone;

namespace RiverFlowProcessor.RiverFlow
{
    public class RiverFlowProcessor : IRiverFlowProcessor
    {
        private readonly IUsgsIvClient usgsIvClient;
        private readonly ILogger<IRiverFlowProcessor> logger;
        private readonly IMetrics metrics;
        private TimerOptions requestTimer;

        public RiverFlowProcessor(
            IUsgsIvClient usgsIvClient,
            ILogger<IRiverFlowProcessor> logger,
            IMetrics metrics)
        {
            this.usgsIvClient = usgsIvClient;
            this.logger = logger;
            this.metrics = metrics;
            this.requestTimer = new TimerOptions
            {
                Name = "River Flow Calls",
                MeasurementUnit = App.Metrics.Unit.Calls,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Minutes
            };
        }

        public async Task Process(string usgsGaugeId)
        {
            usgsGaugeId = Usgs.FormatGaugeId(usgsGaugeId);
            using(metrics.Measure.Timer.Time(requestTimer))
            {
                await GetRiverFlowData(usgsGaugeId);
            }

            // TODO: get other data (weather etc.), call microservice to import/persist
        }

        private async Task GetRiverFlowData(string usgsGaugeId)
        {
            this.logger.LogInformation("Fetching gauge data for site {site}", usgsGaugeId);
            var usgsStreamFlow = await this.usgsIvClient.GetStreamFlow(new[] {usgsGaugeId});

            this.logger.LogDebug("Inspecting, mapping data for site {usgsGaugeId}", usgsGaugeId);
            var riverFlowSnapshot = RiverFlowMapping.MapFlowData(usgsGaugeId, usgsStreamFlow);

            if (riverFlowSnapshot == null)
            {
                this.logger.LogWarning(
                    "No timeseries sensor data returned for gauge {gauge}; skipping.",
                    usgsGaugeId);
                return;
            }

            this.logger.LogInformation("{snapshotSummary}", riverFlowSnapshot);

            // TODO: call service to persist / import flow data
        }
    }

    public interface IRiverFlowProcessor
    {
        Task Process(string usgsGaugeId);
    }
}