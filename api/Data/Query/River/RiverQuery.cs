using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Models.River;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Data.Query.River
{
    public class RiverQuery
        : IRiverQuery
    {
        private readonly RiverDbContext riverDbContext;
        private readonly ILogger<IRiverQuery> logger;
        private readonly IHypermediaService hypermediaService;

        public RiverQuery(
            RiverDbContext riverDbContext,
            ILogger<RiverQuery> logger,
            IHypermediaService hypermediaService)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
            this.hypermediaService = hypermediaService;
        }

        public IQueryable<RiverModel> Query(bool includeGauges)
        {
            var ctx = this.riverDbContext;

            var query = (
                from gauge in ctx.Gauge
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
                join state in ctx.State on gauge.StateCode equals state.StateCode
                group riverGauge.Gauge by new
                {
                    riverGauge.RiverId,
                    river.RiverSection,
                    river.StateCode,
                    river.State.Region,
                    river.State.Division
                } into grp
                select new RiverModel
                {
                    RiverId = grp.Key.RiverId,
                    RiverSection = grp.Key.RiverSection,
                    State = hypermediaService.StateModel(grp.Key.StateCode),
                    Region = grp.Key.Region,
                    Division = grp.Key.Division,
                    Gauges = grp.Select(g => ToGaugeModel(g, includeGauges))
                }
            ).AsQueryable();

            return query;
        }

        private GaugeModel ToGaugeModel(Entities.Gauge gauge, bool includeDetail)
        {
            // TODO: inject dependency that can resolve base url for full absoulte uri
            var url = $"/gauges/{gauge.UsgsGaugeId}";

            if (!includeDetail)
            {
                return new GaugeModel { Href = url };
            }

            var model = new GaugeModel
            {
                Href = url,
                Altitude = gauge.Altitude,
                Lattitude = gauge.Latitude,
                Longitude = gauge.Longitude,
                Name = gauge.Name,
                UsgsGaugeId = gauge.UsgsGaugeId,
                Zone = new SiteZoneInfo
                {
                    DSTZoneAbbrev = gauge.DSTZoneAbbrev,
                    DSTZoneOffset = gauge.DSTZoneOffset,
                    DefaultZoneAbbrev = gauge.DefaultZoneAbbrev,
                    DefaultZoneOffset = gauge.DefaultZoneOffset,
                    ZoneUsesDST = gauge.ZoneUsesDST
                }
            };

            return model;
        }
    }
}