using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Hypermedia;

namespace RiverflowApi.Data.Services
{
    public interface IHypermediaService
    {
        HyperlinkModel Hyperlink(string path, string rel, string method);
    }
}