using System;

namespace RiverFlowApi.Data.DTO
{
    public class StateFlowSummaryDTO
    {
        public class RiverDTO
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return $"{Id} - {Name}";
            }
        }

        public class GaugeDTO
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return $"{Id} - {Name}";
            }
        }

        public class ValueDTO
        {
            public DateTimeOffset AsOf { get; set; }

            public string Code { get; set; }

            public string Name { get; set; }

            public string Unit { get; set; }

            public double Value { get; set; }

            public override string ToString()
            {
                return $"{Name} - {Value} {Unit}";
            }
        }

        public RiverDTO River { get; set; }

        public GaugeDTO Gauge { get; set; }

        public ValueDTO Value { get; set; }

        public override string ToString()
        {
            return $"{River.Name} - {Gauge.Name} - {Value}";
        }
    }
}