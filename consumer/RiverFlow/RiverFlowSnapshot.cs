using System;
using System.Collections.Generic;

namespace RiverFlowProcessor.RiverFlow
{
    public class RiverFlowSnapshot
    {
        public RiverFlowSnapshot()
        {
            this.Values = new List<DataValue>();
        }

        public DateTimeOffset AsOfUTC { get; set; }

        public string AsOfZone { get; set; }

        public List<DataValue> Values { get; set; }

        public string UsgsGaugeId { get; set; }

        public class DataValue
        {
            public DateTimeOffset AsOf { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }

            public string Decription { get; set; }

            public string Unit { get; set; }

            public double Value { get; set; }
        }
    }
}