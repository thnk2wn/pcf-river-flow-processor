using System.Linq;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Factory;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public class GaugeQuery
        : IGaugeQuery
    {
        private readonly RiverDbContext riverDbContext;
        private readonly ILogger<IGaugeQuery> logger;
        private readonly IStateModelFactory stateModelFactory;

        public GaugeQuery(
            RiverDbContext riverDbContext,
            ILogger<IGaugeQuery> logger,
            IStateModelFactory stateModelFactory)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
            this.stateModelFactory = stateModelFactory;
        }

        public IQueryable<GaugeModel> Query(bool includeState)
        {
            var ctx = this.riverDbContext;

            var query = (
                from gauge in ctx.Gauge
                join state in ctx.State on gauge.StateCode equals state.StateCode
                select new GaugeModel
                {
                    Altitude = gauge.Altitude,
                    Lattitude = gauge.Latitude,
                    Longitude = gauge.Longitude,
                    Name = gauge.Name,
                    State = includeState
                        ? this.stateModelFactory.Model(gauge.State)
                        : this.stateModelFactory.Model(gauge.StateCode),
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