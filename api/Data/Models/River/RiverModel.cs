using System.Collections.Generic;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Models.River
{
    public class RiverModel
    {
        public int RiverId { get; set; }

        public string RiverSection { get; set; }

        public StateModel State { get; set; }

        public string Region { get; set; }

        public string Division { get; set; }

        public IEnumerable<GaugeModel> Gauges { get; set; }
    }
}