using System;
using System.Collections.Generic;

namespace RiverFlowApi.Data.Models.Gauge
{
    public class RiverFlowStateSummaryModel
    {
        public RiverFlowStateSummaryModel()
        {
            this.Gauges = new List<GaugeModel>();
        }

        public string River { get; set; }

        public List<GaugeModel> Gauges { get; set; }

        public override string ToString()
        {
            return $"{River} - {Gauges.Count} gauge(s)";
        }

        public class GaugeModel
        {
            public string UsgsGaugeId { get; set; }

            public string Name { get; set; }

            public GaugeReadingModel LatestReading { get; set; }

            public override string ToString()
            {
                return $"{UsgsGaugeId} - {Name}";
            }
        }

        public class GaugeReadingModel
        {
            public DateTimeOffset? AsOf { get; set; }

            public DateTimeOffset AsOfUTC { get; set; }

            public double? HeightFeet { get; set; }

            public double? FlowCFS { get; set; }

            public string UsgsGaugeUrl { get; set; }

            public override string ToString()
            {
                return $"{AsOf} - {HeightFeet} height/ft, {FlowCFS} flow/cfs";
            }
        }
    }
}