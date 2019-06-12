using System.Collections.Generic;
using RiverFlowApi.Data.Models.Hypermedia;

namespace RiverFlowApi.Data.Models.State
{
    public class StateModel
    {
        public List<HyperlinkModel> Links { get; set; }

        public string StateCode { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string Division { get; set; }

        public int? GaugeCount { get; set; }
    }
}