using System.Linq;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.State;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Data.Query.State
{
    public class StateQuery : IStateQuery
    {
        private readonly RiverDbContext riverDbContext;
        private readonly IHypermediaService hypermediaService;

        public StateQuery(
            RiverDbContext riverDbContext,
            IHypermediaService hypermediaService)
        {
            this.riverDbContext = riverDbContext;
            this.hypermediaService = hypermediaService;
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
                select new StateModel
                {
                    Division = state.Division,
                    GaugeCount = stateGaugeCounts.Single(_ => _.StateCode == state.StateCode).GaugeCount,
                    Name = state.Name,
                    Region = state.Region,
                    StateCode = state.StateCode,
                    Links = hypermediaService.StateLinks(state.StateCode)
                }
            ).AsQueryable();

            return query;
        }
    }
}