using System.Collections.Generic;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.Hypermedia;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Factory
{
    public interface IStateModelFactory
    {
        HyperlinkModel Link(string stateCode);

        List<HyperlinkModel> Links(string stateCode);

        StateModel Model(string stateCode);

        StateModel Model(State state, int? gaugeCount);

        StateModel Model(State state);

        StateModel Model(string stateCode, string stateName, string region, string division);
    }
}