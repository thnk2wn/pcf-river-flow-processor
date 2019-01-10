namespace RiverFlowApi.Data
{
    public class UsgsGaugeRiverSection
    {
        public string UsgsGaugeId { get; set; }

        public string RiverSection { get; set; }

        public UsgsGauge Gauge { get; set; }
    }
}