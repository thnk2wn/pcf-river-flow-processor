using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Query
{
    public class StateRiverGaugeQuery
        : ParameterizedQuery<StateRiverGaugeModel, string>, IStateRiverGaugeQuery
    {
        private readonly RiverDbContext riverDbContext;

        public StateRiverGaugeQuery(RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
        }

        protected override async Task<IEnumerable<StateRiverGaugeModel>> QueryAsync(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var ctx = this.riverDbContext;

            var models = await (
                from gauge in ctx.Gauge
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
                where river.StateCode == state
                group riverGauge.Gauge by new {riverGauge.RiverId, river.RiverSection} into grp
                select new StateRiverGaugeModel
                {
                    RiverId = grp.Key.RiverId,
                    RiverSection = grp.Key.RiverSection,
                    Gauges = grp.Select(g => new StateRiverGaugeModel.GaugeModel
                    {
                        Altitude = g.Altitude,
                        Lattitude = g.Latitude,
                        Longitude = g.Longitude,
                        Name = g.Name,
                        UsgsGaugeId = g.UsgsGaugeId,
                        Zone = new StateRiverGaugeModel.SiteZoneInfo
                        {
                            DSTZoneAbbrev = g.DSTZoneAbbrev,
                            DSTZoneOffset = g.DSTZoneOffset,
                            DefaultZoneAbbrev = g.DefaultZoneAbbrev,
                            DefaultZoneOffset = g.DefaultZoneOffset,
                            ZoneUsesDST = g.ZoneUsesDST
                        }
                    })
                    .ToList()
                }
            ).ToListAsync();

            return models;
        }
    }
}