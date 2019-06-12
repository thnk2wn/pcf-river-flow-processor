using System.Collections.Generic;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.Hypermedia;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Factory
{
    public class StateModelFactory : IStateModelFactory
    {
        private readonly IHypermediaService hypermediaService;

        public StateModelFactory(IHypermediaService hypermediaService)
        {
            this.hypermediaService = hypermediaService;
        }

        public HyperlinkModel Link(string stateCode)
        {
            var linkModel = this.hypermediaService.Hyperlink(
                path: $"states/{stateCode}",
                rel: "states",
                method: "GET");
            return linkModel;
        }

        public List<HyperlinkModel> Links(string stateCode)
        {
            return new List<HyperlinkModel> { this.Link(stateCode) };
        }

        public StateModel Model(string stateCode)
        {
            return new StateModel
            {
                Links = this.Links(stateCode),
                StateCode = stateCode
            };
        }

        public StateModel Model(
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
                Links = this.Links(state.StateCode),
                Name = state.Name,
                Region = state.Region,
                StateCode = state.StateCode
            };
            return model;
        }

        public StateModel Model(State state)
        {
            return this.Model(state, null);
        }

        public StateModel Model(string stateCode, string stateName, string region, string division)
        {
            var stateModel = new StateModel
            {
                Division = division,
                Links = this.Links(stateCode),
                Name = stateName,
                Region = region,
                StateCode = stateCode
            };
            return stateModel;
        }
    }
}