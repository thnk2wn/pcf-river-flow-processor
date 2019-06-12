using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Models.Gauge
{
    public class GaugeModel
    {
        public string Href { get; set; }

        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public decimal? Lattitude { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Altitude { get; set; }

        public SiteZoneInfo Zone { get; set; }

        public StateModel State { get; set; }
    }
}