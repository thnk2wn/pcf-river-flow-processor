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
using Polly;
using RiverFlow.Shared;
using RiverFlowProcessor.USGS;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using TimeZone = RiverFlowProcessor.USGS.TimeZone;

namespace RiverFlowProcessor.RiverFlow
{
    public class RiverFlowProcessor : IRiverFlowProcessor
    {
        private readonly IUsgsIvClient usgsIvClient;
        private readonly IFlowClient flowClient;
        private readonly ILogger<IRiverFlowProcessor> logger;
        private readonly IMetrics metrics;
        private TimerOptions requestTimer;
        private TimerOptions recordTimer;

        public RiverFlowProcessor(
            IUsgsIvClient usgsIvClient,
            IFlowClient flowClient,
            ILogger<IRiverFlowProcessor> logger,
            IMetrics metrics)
        {
            this.usgsIvClient = usgsIvClient;
            this.flowClient = flowClient;
            this.logger = logger;
            this.metrics = metrics;

            this.requestTimer = new TimerOptions
            {
                Name = "River Flow Queries",
                MeasurementUnit = App.Metrics.Unit.Calls,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Minutes
            };

            this.recordTimer = new TimerOptions
            {
                Name = "River Flow Recordings",
                MeasurementUnit = App.Metrics.Unit.Calls,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Minutes
            };
        }

        public async Task Process(string usgsGaugeId)
        {
            usgsGaugeId = Usgs.FormatGaugeId(usgsGaugeId);
            RiverFlowSnapshot flowData = null;

            using (metrics.Measure.Timer.Time(this.requestTimer))
            {
                flowData = await this.GetRiverFlowData(usgsGaugeId);
            }

            if (flowData != null)
            {
                using (metrics.Measure.Timer.Time(this.recordTimer))
                {
                    await this.flowClient.RecordFlow(flowData);
                }
            }
        }

        private async Task<RiverFlowSnapshot> GetRiverFlowData(string usgsGaugeId)
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
                return null;
            }

            this.logger.LogInformation("{snapshotSummary}", riverFlowSnapshot);
            return riverFlowSnapshot;
        }
    }

    public interface IRiverFlowProcessor
    {
        Task Process(string usgsGaugeId);
    }
}