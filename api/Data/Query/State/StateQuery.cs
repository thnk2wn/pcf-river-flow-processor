using System.Linq;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Factory;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Query.State
{
    public class StateQuery : IStateQuery
    {
        private readonly RiverDbContext riverDbContext;
        private readonly IStateModelFactory stateModelFactory;

        public StateQuery(
            RiverDbContext riverDbContext,
            IStateModelFactory stateModelFactory)
        {
            this.riverDbContext = riverDbContext;
            this.stateModelFactory = stateModelFactory;
        }

        public IQueryable<StateModel> Query()
        {
            var ctx = this.riverDbContext;

            var stateGaugeCounts = (
                from gauge in ctx.Gauge
                group gauge.State by new {gauge.StateCode} into grp
                select new
                {
                    StateCode = grp.Key.StateCode,
                    GaugeCount = grp.Count()
                }
            ).ToList();

            var query = (
                from state in ctx.State
                orderby state.Name
                select this.stateModelFactory.Model(
                    state,
                    stateGaugeCounts.Single(_ => _.StateCode == state.StateCode).GaugeCount)
            ).AsQueryable();

            return query;
        }
    }
}