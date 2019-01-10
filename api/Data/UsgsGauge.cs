using System.Collections.Generic;

namespace RiverFlowApi.Data
{
    public class UsgsGauge
    {
        public UsgsGauge()
        {
            this.RiverSections = new List<UsgsGaugeRiverSection>();
            this.Flows = new List<UsgsGaugeFlow>();
        }

        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public string StateCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal? Altitude { get; set; }

        public State State { get; set; }

        public virtual ICollection<UsgsGaugeRiverSection> RiverSections { get; set; }

        public virtual ICollection<UsgsGaugeFlow> Flows { get; set; }
    }
}