namespace RiverFlowApi.Data
{
    public class Gauge
    {
        public string UsgsGaugeId { get; set; }

        public string UsgsGaugeName { get; set; }

        public string StateCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal Altitude { get; set; }
    }
}