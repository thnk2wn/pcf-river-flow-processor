using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IStateRiverGaugeQuery> logger;

        public StateRiverGaugeQuery(
            RiverDbContext riverDbContext,
            ILogger<IStateRiverGaugeQuery> logger)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
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

        protected override void OnAfterQueryFailure(AfterQueryFailureEventArgs<string> e)
        {
            this.logger.LogWarning(
                e.Error,
                "Failed to query rivers by state {state}. Duration: {time}",
                e.Param,
                e.ElapsedText);
        }

        protected override void OnAfterQuerySuccess(
            AfterQuerySuccessEventArgs<StateRiverGaugeModel, string> e)
        {
            this.logger.LogInformation(
                "Retrieved {count} river records for {state}. Duration: {time}",
                e.Results.Count(),
                e.Param,
                e.ElapsedText);
        }
    }
}