using System.Collections.Generic;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Services
{
    public static class IHypermediaServiceExtensions
    {
        public static HyperlinkModel StateLink(this IHypermediaService service, string stateCode)
        {
            var linkModel = service.Hyperlink(
                path: $"states/{stateCode}",
                rel: "states",
                method: "GET");
            return linkModel;
        }

        public static Models.State.StateModel StateModel(this IHypermediaService service, string stateCode)
        {
            return new Models.State.StateModel
            {
                Links = new List<HyperlinkModel> { service.StateLink(stateCode) },
                StateCode = stateCode
            };
        }
    }
}