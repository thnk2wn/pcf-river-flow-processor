using RiverFlowApi.Data.Models;

namespace RiverflowApi.Data.Services
{
    public interface IHypermediaService
    {
        HyperlinkModel Hyperlink(string path, string rel, string method);
    }
}