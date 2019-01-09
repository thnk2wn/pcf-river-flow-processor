namespace RiverFlowApi.Data
{
    public class UsgsGauge
    {
        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public string StateCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal? Altitude { get; set; }

        public State State { get; set; }
    }
}