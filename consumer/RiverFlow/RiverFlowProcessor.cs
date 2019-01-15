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
            var streamFlow = await this.usgsIvClient.GetStreamFlow(new[] {usgsGaugeId});

            this.logger.LogDebug("Inspecting, mapping data for site {usgsGaugeId}", usgsGaugeId);

            // Current assumptions for simplicity
            // 1) One site at a time though API supports multiple
            // 2) One parameter value at a time (most recent) though API supports time period

            var timeSeries = streamFlow.Value.GetTimeSeriesForSite(usgsGaugeId);

            var snapshot = new RiverFlowSnapshot
            {
                AsOfUTC = DateTime.UtcNow,
                AsOfZone = System.TimeZoneInfo.Local.StandardName,
                UsgsGaugeId = usgsGaugeId
            };

            foreach (var ts in timeSeries.Where(ts => ts.Values?.Length == 1 && ts.Values[0].Value?.Length == 1))
            {
                var tsValue = ts.Values[0].Value[0];
                var parsed = double.TryParse(tsValue.Value, out double value);

                if (parsed && ts.Variable.NoDataValue != (long)value)
                {
                    // System.TimeZoneInfo.GetSystemTimeZones()[0].BaseUtcOffset
                    // System.TimeZoneInfo.ConvertTimeToUtc(tsValue.DateTime, TimeZoneInfo.)
                    var dataValue = new RiverFlowSnapshot.DataValue
                    {
                        AsOf = tsValue.DateTime,
                        Code = ts.Variable.VariableCode[0].Value,
                        Decription = ts.Variable.VariableDescription,
                        Name = System.Net.WebUtility.HtmlDecode(ts.Variable.VariableName),
                        Unit = ts.Variable.Unit.UnitCode,
                        Value = value
                    };
                    snapshot.Values.Add(dataValue);
                }
            }

            if (!snapshot.Values.Any())
            {
                this.logger.LogWarning(
                    "No timeseries sensor data returned for gauge {gauge}; skipping.",
                    usgsGaugeId);
                return;
            }

            this.logger.LogInformation("{snapshotSummary}", snapshot);

            // TODO: call service to persist / import flow data
        }
    }

    public interface IRiverFlowProcessor
    {
        Task Process(string usgsGaugeId);
    }
}