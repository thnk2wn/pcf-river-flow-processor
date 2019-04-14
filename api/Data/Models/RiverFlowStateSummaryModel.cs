using System.Collections.Generic;

namespace RiverFlowApi.Data.Models
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

            public double? HeightFeet { get; set; }

            public double? FlowCFS { get; set; }

            public string AsOfDuration { get; set; }

            public override string ToString()
            {
                return $"{UsgsGaugeId} - {Name}";
            }
        }
    }
}