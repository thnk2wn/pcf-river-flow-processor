using System;
using System.Collections.Generic;

namespace RiverFlowApi.Data.Entities
{
    public class GaugeReport
    {
        public GaugeReport()
        {
            this.GaugeValues = new List<GaugeValue>();
        }

        public int ReportId { get; set; }

        public string UsgsGaugeId { get; set; }

        public bool Latest { get; set; }

        public DateTime AsOf { get; set; }

        public DateTime AsOfUTC { get; set; }

        public int GaugeValueCount { get; set; }

        public virtual ICollection<GaugeValue> GaugeValues { get; set; }

        public string InstanceId { get; set; }

        public Gauge Gauge { get; set; }
    }
}