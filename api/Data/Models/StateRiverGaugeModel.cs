using System.Collections.Generic;

namespace RiverFlowApi.Data.Models
{
    public class StateRiverGaugeModel
    {
        public int RiverId { get; set; }

        public string RiverSection { get; set; }

        public List<GaugeModel> Gauges { get; set; }

        public class GaugeModel
        {
            public string UsgsGaugeId { get; set; }

            public string Name { get; set; }

            public decimal Lattitude { get; set; }

            public decimal Longitude { get; set; }

            public decimal? Altitude { get; set; }
        }
    }
}