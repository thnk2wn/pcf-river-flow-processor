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
                Name = "River Flow Requests",
                MeasurementUnit = App.Metrics.Unit.Requests,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Seconds
            };
        }

        public async Task Process(string usgsGaugeId)
        {
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

            var gaugeHeight = streamFlow.GetLastTimeSeriesValue(UsgsVariables.GaugeHeightFeet);
            var dischargeCfs = streamFlow.GetLastTimeSeriesValue(UsgsVariables.DischargeCFS);
            var waterTemp = streamFlow.GetLastTimeSeriesValue(UsgsVariables.WaterTempCelsius);
            var firstSetSeries = (gaugeHeight ?? dischargeCfs ?? waterTemp);

            if (firstSetSeries == null) 
            {
                this.logger.LogWarning(
                    "No timeseries sensor data returned for gauge {gauge}; skipping.", 
                    usgsGaugeId);
                return;
            }

            var sourceSite = streamFlow.GetSource();

            var snapshot = new RiverFlowSnapshot 
            {
                 AsOf = DateTime.UtcNow,

                 Flow = new RiverFlowSnapshot.FlowValues 
                 {
                     AsOf = (firstSetSeries).DateTime,
                     GaugeHeightFeet = GetFlowValue(gaugeHeight),
                     DischargeCFS = GetFlowValue(dischargeCfs),
                     WaterTemperature = GetTemp(waterTemp)
                 },

                 Site = new RiverFlowSnapshot.SourceSite 
                 {
                     Code = usgsGaugeId,
                     Name = sourceSite.SiteName,
                     Latitude = sourceSite.GeoLocation.GeogLocation.Latitude,
                     Longitude = sourceSite.GeoLocation.GeogLocation.Longitude
                 }
            };

            this.logger.LogInformation("{snapshotSummary}", snapshot);

            // TODO: call service to persist / import flow data
        }

        private double? GetFlowValue(TimeSeriesValue timeSeriesValue) 
        {
            if (timeSeriesValue != null && double.TryParse(timeSeriesValue.Value, out double value)) 
            {
                return value;
            }

            return null;
        }

        private RiverFlowSnapshot.Temperature GetTemp(TimeSeriesValue timeSeriesValue) 
        {
            var value = GetFlowValue(timeSeriesValue);

            return value == null ? null : new RiverFlowSnapshot.Temperature 
            {
                Celsius = value.Value, 
                Fahrenheit = value.Value * 9 / 5 + 32
            };
        }
    }

    public interface IRiverFlowProcessor
    {
        Task Process(string usgsGaugeId);
    }
}