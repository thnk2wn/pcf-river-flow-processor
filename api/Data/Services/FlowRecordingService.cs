using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using static RiverFlowApi.Data.Models.RiverFlowSnapshotModel;

namespace RiverFlowApi.Data.Services
{
    public class FlowRecordingService : IFlowRecordingService
    {
        private RiverDbContext riverDbContext;

        public FlowRecordingService(RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
        }

        public async Task Record(RiverFlowSnapshotModel snapshot)
        {
            foreach (var dataValue in snapshot.Values)
            {
                await this.RecordValue(dataValue, snapshot);
            }

            await this.riverDbContext.SaveChangesAsync();
        }

        private async Task RecordValue(DataValue value, RiverFlowSnapshotModel snapshot)
        {
            await this.EnsureGauge(snapshot.Site);
            await this.EnsureVariable(value);
            await this.AddGaugeValue(value, snapshot);
        }

        private async Task EnsureVariable(DataValue value)
        {
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

        private async Task EnsureGauge(SiteInfo siteInfo)
        {
            var gauge = await this.riverDbContext.Gauge.SingleAsync(g => g.UsgsGaugeId == siteInfo.UsgsGaugeId);

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
        }
    }
}