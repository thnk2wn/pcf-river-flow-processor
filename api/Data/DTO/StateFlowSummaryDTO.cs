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

            public string TimeZoneAbbrev { get; set; }

            public override string ToString()
            {
                return $"{Id} - {Name}";
            }
        }

        public class ReportDTO
        {
            public DateTimeOffset AsOf { get; set; }

            public DateTimeOffset AsOfUTC { get; set; }
        }

        public class ValueDTO
        {
            public DateTime AsOf { get; set; }

            public DateTime AsOfUTC { get; set; }

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

        public ReportDTO Report { get; set; }

        public override string ToString()
        {
            return $"{River.Name} - {Gauge.Name} - {Value}";
        }
    }
}