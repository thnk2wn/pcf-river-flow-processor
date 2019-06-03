using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Entities;

namespace RiverFlowApi.Data.Query
{
    public class StateRiverGaugeQuery
        : ParameterizedQuery<StateRiverGaugeDTO, string>, IStateRiverGaugeQuery
    {
        private readonly RiverDbContext riverDbContext;

        public StateRiverGaugeQuery(RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
        }

        protected override async Task<IEnumerable<StateRiverGaugeDTO>> QueryAsync(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var ctx = this.riverDbContext;

            var riverGaugeDtos = await (
                from gauge in ctx.Gauge
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
                orderby river.RiverSection, gauge.Name
                where river.StateCode == state
                select new StateRiverGaugeDTO
                {
                    Altitude = gauge.Altitude,
                    Lattitude = gauge.Latitude,
                    Longitude = gauge.Longitude,
                    Name = gauge.Name,
                    RiverId = river.RiverId,
                    RiverSection = river.RiverSection,
                    UsgsGaugeId = gauge.UsgsGaugeId
                }
            ).ToListAsync();

            return riverGaugeDtos;
        }
    }
}