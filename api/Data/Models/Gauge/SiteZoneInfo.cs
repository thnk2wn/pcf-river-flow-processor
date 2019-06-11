namespace RiverFlowApi.Data.Models.Gauge
{
    public class SiteZoneInfo
    {
        public string DefaultZoneOffset { get; set; }

        public string DefaultZoneAbbrev { get; set; }

        public string DSTZoneOffset { get; set; }

        public string DSTZoneAbbrev { get; set; }

        public bool? ZoneUsesDST { get; set; }
    }
}