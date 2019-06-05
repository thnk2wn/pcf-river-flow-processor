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
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public class StateGaugeQuery
        : ParameterizedQuery<StateGaugeModel, string>, IStateGaugeQuery
    {
        private readonly ILogger<IStateGaugeQuery> logger;

        public StateGaugeQuery(
            RiverDbContext riverDbContext,
            ILogger<IStateGaugeQuery> logger)
            : base(riverDbContext)
        {
            this.logger = logger;
        }

        protected override async Task<IEnumerable<StateGaugeModel>> QueryAsync(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var ctx = this.RiverDbContext;

            var models = await (
                from gauge in ctx.Gauge
                where gauge.StateCode == state
                select new StateGaugeModel
                {
                    Altitude = gauge.Altitude,
                    Lattitude = gauge.Latitude,
                    Longitude = gauge.Longitude,
                    Name = gauge.Name,
                    UsgsGaugeId = gauge.UsgsGaugeId,
                    Zone = new StateGaugeModel.SiteZoneInfo
                    {
                        DSTZoneAbbrev = gauge.DSTZoneAbbrev,
                        DSTZoneOffset = gauge.DSTZoneOffset,
                        DefaultZoneAbbrev = gauge.DefaultZoneAbbrev,
                        DefaultZoneOffset = gauge.DefaultZoneOffset,
                        ZoneUsesDST = gauge.ZoneUsesDST
                    }
                }
            ).ToListAsync();

            return models;
        }

        protected override void OnAfterQueryFailure(AfterQueryFailureEventArgs<string> e)
        {
            this.logger.LogWarning(
                e.Error,
                "Failed to query gauges by state {state}. Duration: {time}",
                e.Param,
                e.ElapsedText);
        }

        protected override void OnAfterQuerySuccess(
            AfterQuerySuccessEventArgs<StateGaugeModel, string> e)
        {
            this.logger.LogInformation(
                "Retrieved {count} gauge records for {state}. Duration: {time}",
                e.Results.Count(),
                e.Param,
                e.ElapsedText);
        }
    }
}