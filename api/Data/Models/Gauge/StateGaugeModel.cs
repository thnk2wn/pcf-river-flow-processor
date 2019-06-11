using System.Collections.Generic;

namespace RiverFlowApi.Data.Models.Gauge
{
    public class StateGaugeModel
    {
        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public decimal Lattitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal? Altitude { get; set; }

        public SiteZoneInfo Zone { get; set; }
    }
}