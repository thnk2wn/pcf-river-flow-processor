using System.Collections.Generic;

namespace RiverFlowApi.Data.Entities
{
    public class Gauge
    {
        public Gauge()
        {
            this.Values = new List<GaugeValue>();
        }

        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public string StateCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal? Altitude { get; set; }

        public State State { get; set; }

        public virtual ICollection<GaugeValue> Values { get; set; }
    }
}