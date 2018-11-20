using System;

namespace RiverFlowProcessor.RiverFlow
{
    public class RiverFlowSnapshot
    {
        public DateTimeOffset AsOf { get; set; }

        public FlowValues Flow { get; set; }

        public SourceSite Site { get; set; }

        public override string ToString()
        {
            return $"{Site?.Code} - {Site?.Name}: " +
                $"{Flow?.GaugeHeightFeet} ft, {Flow?.DischargeCFS} cfs, {Flow?.WaterTemperature?.Fahrenheit} F";
        }

        public class SourceSite 
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }

        public class FlowValues
        {
            public DateTimeOffset AsOf { get; set; }

            public double? DischargeCFS { get; set; }

            public double? GaugeHeightFeet { get; set; }

            public Temperature WaterTemperature { get; set; }
        }

        public class Temperature 
        {
            public double Celsius { get; set; }

            public double Fahrenheit { get; set; }
        }
    }
}