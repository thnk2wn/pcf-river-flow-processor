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

        public class SiteZoneInfo
        {
            public string DefaultZoneOffset { get; set; }

            public string DefaultZoneAbbrev { get; set; }

            public string DSTZoneOffset { get; set; }

            public string DSTZoneAbbrev { get; set; }

            public bool? ZoneUsesDST { get; set; }
        }
    }
}