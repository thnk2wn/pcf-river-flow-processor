using System;
using System.Collections.Generic;

namespace RiverFlowApi.Data.Models
{
    public class RiverFlowSnapshotModel
    {
        public RiverFlowSnapshotModel()
        {
            this.Values = new List<DataValue>();
        }

        public DateTimeOffset AsOfUTC { get; set; }

        public List<DataValue> Values { get; set; }

        public SiteInfo Site { get; set; }

        public class DataValue
        {
            public DateTimeOffset AsOf { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }

            public string Decription { get; set; }

            public string Unit { get; set; }

            public double Value { get; set; }
        }

        public partial class SiteInfo
        {
            public string UsgsGaugeId { get; set; }

            public SiteTimeZone DefaultTimeZone { get; set; }

            public SiteTimeZone DaylightSavingsTimeZone { get; set; }

            public bool UsesDaylightSavingsTime { get; set; }
        }

        public partial class SiteTimeZone
        {
            public string ZoneOffset { get; set; }

            public string ZoneAbbreviation { get; set; }
        }
    }
}