using System.Linq;
using Microsoft.Extensions.Logging;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Data.Query.Gauge
{
    public class GaugeQuery
        : IGaugeQuery
    {
        private readonly RiverDbContext riverDbContext;
        private readonly ILogger<IGaugeQuery> logger;
        private readonly IHypermediaService hypermediaService;

        public GaugeQuery(
            RiverDbContext riverDbContext,
            ILogger<IGaugeQuery> logger,
            IHypermediaService hypermediaService)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
            this.hypermediaService = hypermediaService;
        }

        public IQueryable<GaugeModel> Query()
        {
            var ctx = this.riverDbContext;

            var query = (
                from gauge in ctx.Gauge
                select new GaugeModel
                {
                    Altitude = gauge.Altitude,
                    Lattitude = gauge.Latitude,
                    Longitude = gauge.Longitude,
                    Name = gauge.Name,
                    State = hypermediaService.StateModel(gauge.StateCode),
                    UsgsGaugeId = gauge.UsgsGaugeId,
                    Zone = new SiteZoneInfo
                    {
                        DSTZoneAbbrev = gauge.DSTZoneAbbrev,
                        DSTZoneOffset = gauge.DSTZoneOffset,
                        DefaultZoneAbbrev = gauge.DefaultZoneAbbrev,
                        DefaultZoneOffset = gauge.DefaultZoneOffset,
                        ZoneUsesDST = gauge.ZoneUsesDST
                    }
                }
            ).AsQueryable();

            return query;
        }
    }
}