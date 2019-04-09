using System.Collections.Generic;

namespace RiverFlowApi.Data.Models
{
    public class RiverFlowStateSummaryModel
    {
        public RiverFlowStateSummaryModel()
        {
            this.Rivers = new List<RiversModel>();
        }

        public List<RiversModel> Rivers { get; set; }

        public class RiversModel
        {
            public RiversModel()
            {
                this.Gauges = new List<GaugeModel>();
            }

            public string River { get; set; }

            public List<GaugeModel> Gauges { get; set; }
        }

        public class GaugeModel
        {
            public string UsgsGaugeId { get; set; }

            public string Name { get; set; }

            public double? HeightFeet { get; set; }

            public double? FlowCFS { get; set; }

            public string AsOfDuration { get; set; }
        }
    }
}