using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Query.State
{
    public class StateQuery : ParameterizedQuery<StateModel, NoOp>, IStateQuery
    {
        public StateQuery(
            RiverDbContext riverDbContext
        ) : base(riverDbContext)
        {
        }

        public async Task<List<StateModel>> RunListAsync()
        {
            return await this.RunListAsync(new NoOp());
        }

        protected override async Task<IEnumerable<StateModel>> QueryAsync(NoOp param)
        {
            var ctx = this.RiverDbContext;

            var stateGaugeCounts = await (
                from gauge in ctx.Gauge
                group gauge.State by new {gauge.StateCode} into grp
                select new
                {
                    StateCode = grp.Key.StateCode,
                    GaugeCount = grp.Count()
                }
            ).ToListAsync();

            var states = await (
                from state in ctx.State
                orderby state.Name
                select new StateModel
                {
                    Division = state.Division,
                    GaugeCount = stateGaugeCounts.Single(_ => _.StateCode == state.StateCode).GaugeCount,
                    Name = state.Name,
                    Region = state.Region,
                    StateCode = state.StateCode
                }
            ).ToListAsync();

            return states;
        }
    }
}