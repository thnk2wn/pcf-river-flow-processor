using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using static RiverFlowApi.Data.Models.RiverFlowSnapshotModel;

namespace RiverFlowApi.Data.Services
{
    public class FlowRecordingService : IFlowRecordingService
    {
        private readonly RiverDbContext riverDbContext;
        private readonly ILogger<IFlowRecordingService> logger;
        private HashSet<string> variableCodesChecked;

        public FlowRecordingService(ILogger<IFlowRecordingService> logger, RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
            this.variableCodesChecked = new HashSet<string>();
        }

        public async Task Record(RiverFlowSnapshotModel snapshot)
        {
            if (!await this.EnsureGauge(snapshot.Site))
            {
                return;
            }

            foreach (var dataValue in snapshot.Values)
            {
                await this.RecordValue(dataValue, snapshot);
            }

            await this.riverDbContext.SaveChangesAsync();
        }

        private async Task RecordValue(DataValue value, RiverFlowSnapshotModel snapshot)
        {
            await this.EnsureVariable(value);
            await this.AddGaugeValue(value, snapshot);
        }

        private async Task EnsureVariable(DataValue value)
        {
            if (this.variableCodesChecked.Contains(value.Code))
            {
                return;
            }

            if (!await this.riverDbContext.Variable.AnyAsync(v => v.Code == value.Code))
            {
                var variable = new Variable
                {
                    Code = value.Code,
                    Description = value.Decription,
                    Name = value.Name,
                    Unit = value.Unit
                };
                await this.riverDbContext.Variable.AddAsync(variable);
                this.variableCodesChecked.Add(value.Code);
            }
        }

        private async Task AddGaugeValue(DataValue value, RiverFlowSnapshotModel snapshot)
        {
            var exists = await this.riverDbContext
                .GaugeValue
                .AnyAsync(gv => gv.AsOfUTC == snapshot.AsOfUTC
                    && gv.UsgsGaugeId == snapshot.Site.UsgsGaugeId
                    && gv.Code == value.Code);

            if (!exists)
            {
                var gaugeValue = new GaugeValue
                {
                    AsOf = value.AsOf,
                    AsOfUTC = snapshot.AsOfUTC,
                    Code = value.Code,
                    UsgsGaugeId = snapshot.Site.UsgsGaugeId,
                    Value = value.Value
                };
                await this.riverDbContext.GaugeValue.AddAsync(gaugeValue);
            }
        }

        private async Task<bool> EnsureGauge(SiteInfo siteInfo)
        {
            var gauge = await this.riverDbContext.Gauge.SingleOrDefaultAsync(g => g.UsgsGaugeId == siteInfo.UsgsGaugeId);

            if (gauge == null)
            {
                this.logger.LogWarning(
                    "Gauge {usgsGaugeId} not found in table. Either missing from data seeding or unknown queue guage id",
                    siteInfo.UsgsGaugeId);
                return false;
            }

            if (gauge.DefaultZoneOffset == null ||
                gauge.DefaultZoneAbbrev == null ||
                gauge.DSTZoneOffset == null ||
                gauge.DSTZoneAbbrev == null)
            {
                gauge.DefaultZoneAbbrev = siteInfo.DefaultTimeZone.ZoneAbbreviation;
                gauge.DefaultZoneOffset = siteInfo.DefaultTimeZone.ZoneOffset;
                gauge.DSTZoneAbbrev = siteInfo.DaylightSavingsTimeZone.ZoneAbbreviation;
                gauge.DSTZoneOffset = siteInfo.DaylightSavingsTimeZone.ZoneOffset;
                gauge.ZoneUsesDST = siteInfo.UsesDaylightSavingsTime;
            }

            return true;
        }
    }
}