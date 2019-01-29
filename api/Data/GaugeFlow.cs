namespace RiverFlowApi.Data
{
    using System;

    public class GaugeFlow
    {
        public int FlowId { get; set; }

        public string UsgsGaugeId { get; set; }

        public DateTime AsOf { get; set; }

        public DateTime AddedDate { get; set; }

        public double? GaugeHeightFeet { get; set; }

        public double? DischargeCFS { get; set; }

        public double? WaterTempF { get; set; }

        public double? WaterTempC { get; set; }

        public Gauge Gauge { get; set; }
    }
}