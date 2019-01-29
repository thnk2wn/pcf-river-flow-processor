namespace RiverFlowApi.Data
{
    public class RiverGauge
    {
        public int RiverId { get; set; }

        public string UsgsGaugeId { get; set; }

        public virtual River River { get; set; }

        public virtual Gauge Gauge { get; set; }
    }
}