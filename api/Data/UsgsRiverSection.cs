namespace RiverFlowApi.Data
{
    public class UsgsRiverSection
    {
        public string UsgsGaugeId { get; set; }

        public string RiverName { get; set; }

        public UsgsGauge Gauge { get; set; }
    }
}