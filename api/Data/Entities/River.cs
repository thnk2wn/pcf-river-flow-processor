namespace RiverFlowApi.Data.Entities
{
    public class River
    {
        public int RiverId { get; set; }

        public string RiverSection { get; set; }

        public string StateCode { get; set; }

        public State State { get; set; }
    }
}