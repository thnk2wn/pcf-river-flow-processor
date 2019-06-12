using System.Collections.Generic;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.State;

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

        public static List<HyperlinkModel> StateLinks(this IHypermediaService service, string stateCode)
        {
            return new List<HyperlinkModel> { service.StateLink(stateCode) };
        }

        public static StateModel StateModel(this IHypermediaService service, string stateCode)
        {
            return new StateModel
            {
                Links = service.StateLinks(stateCode),
                StateCode = stateCode
            };
        }

        // TODO: consider moving this to separate mapping class
        public static StateModel StateModel(
            this IHypermediaService service,
            State state,
            int? gaugeCount)
        {
            if (state == null)
            {
                return null;
            }

            var model = new StateModel
            {
                Division = state.Division,
                GaugeCount = gaugeCount,
                Links = service.StateLinks(state.StateCode),
                Name = state.Name,
                Region = state.Region,
                StateCode = state.StateCode
            };
            return model;
        }

        public static StateModel StateModel(
            this IHypermediaService service,
            State state)
        {
            return service.StateModel(state, null);
        }
    }
}