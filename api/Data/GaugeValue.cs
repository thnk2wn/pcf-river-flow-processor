using System;

namespace RiverFlowApi.Data
{
    public class GaugeValue
    {
        public DateTimeOffset AsOfUTC { get; set; }

        public DateTimeOffset AsOf {get; set; }

        public string UsgsGaugeId { get; set; }

        public string Code { get; set; }

        public double Value { get; set; }

        public Gauge Gauge { get; set; }

        public Variable Variable { get; set; }
    }
}