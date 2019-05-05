using System;

namespace RiverFlowApi.Data.Entities
{
    public class GaugeValue
    {
        public DateTime AsOf {get; set; }

        public string UsgsGaugeId { get; set; }

        public int ReportId { get; set; }

        public string Code { get; set; }

        public double Value { get; set; }

        public Gauge Gauge { get; set; }

        public Variable Variable { get; set; }

        public GaugeReport Report { get; set; }
    }
}