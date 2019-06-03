namespace RiverFlowApi.Data.DTO
{
    public class StateRiverGaugeDTO
    {
        public int RiverId { get; set; }

        public string RiverSection { get; set; }

        public string UsgsGaugeId { get; set; }

        public string Name { get; set; }

        public decimal Lattitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal? Altitude { get; set; }
    }
}