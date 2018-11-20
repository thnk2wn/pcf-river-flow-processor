using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public RiverFlowProcessor(
            IUsgsIvClient usgsIvClient,
            ILogger<IRiverFlowProcessor> logger) 
        {
            this.usgsIvClient = usgsIvClient;
            this.logger = logger;
        }

        public async Task Process(string usgsGaugeId)
        {
            this.logger.LogInformation("Fetching gauge data");
            var streamFlow = await this.usgsIvClient.GetStreamFlow(new[] {usgsGaugeId});
            this.logger.LogInformation("Fetched gauge data");

            this.logger.LogInformation("Inspecting, mapping data for gauge {usgsGaugeId}", usgsGaugeId);

            var gaugeHeight = streamFlow.GetLastTimeSeriesValue(UsgsVariables.GaugeHeightFeet);
            var dischargeCfs = streamFlow.GetLastTimeSeriesValue(UsgsVariables.DischargeCFS);
            var waterTemp = streamFlow.GetLastTimeSeriesValue(UsgsVariables.WaterTempCelsius);
            var firstSetSeries = (gaugeHeight ?? dischargeCfs ?? waterTemp);
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

            this.logger.LogDebug("Data mapped for gauge {usgsGaugeId}", usgsGaugeId);
            this.logger.LogInformation("{snapshotSummary}", snapshot);
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