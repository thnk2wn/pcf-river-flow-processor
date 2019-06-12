namespace RiverFlowApi.Data.Models.Hypermedia
{
    public class HyperlinkModel
    {
        public string Href { get; }

        public string Rel { get; }

        public string Method { get; }

        public HyperlinkModel(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }

        public static HyperlinkModel Get(string href, string rel)
        {
            return new HyperlinkModel(href, rel, "GET");
        }

        public static HyperlinkModel Post(string href, string rel)
        {
            return new HyperlinkModel(href, rel, "POST");
        }
    }
}