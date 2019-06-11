using System.Collections.Generic;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Models.River
{
    public class RiverModel
    {
        public int RiverId { get; set; }

        public string RiverSection { get; set; }

        public string StateCode { get; set; }

        public string Region { get; set; }

        public string Division { get; set; }

        public IEnumerable<GaugeModel> Gauges { get; set; }

        public class GaugeModel
        {
            public string Href { get; set; }

            public string UsgsGaugeId { get; set; }

            public string Name { get; set; }

            public decimal? Lattitude { get; set; }

            public decimal? Longitude { get; set; }

            public decimal? Altitude { get; set; }

            public SiteZoneInfo Zone { get; set; }
        }
    }
}