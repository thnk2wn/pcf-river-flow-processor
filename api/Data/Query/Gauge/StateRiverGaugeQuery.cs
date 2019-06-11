using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public class StateRiverGaugeQuery : IStateRiverGaugeQuery
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

        public IQueryable<StateRiverGaugeModel> Query()
        {
            var ctx = this.riverDbContext;

            var models = (
                from gauge in ctx.Gauge
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
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
            ).AsQueryable();

            return models;
        }
    }
}