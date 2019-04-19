using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Configuration;
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
        private string appInstanceId;

        public FlowRecordingService(
            ILogger<IFlowRecordingService> logger,
            RiverDbContext riverDbContext,
            IConfiguration configuration)
        {
            this.riverDbContext = riverDbContext;
            this.logger = logger;
            this.variableCodesChecked = new HashSet<string>();
            this.appInstanceId = configuration.GetAppInstanceId();
        }

        public async Task Record(RiverFlowSnapshotModel snapshot)
        {
            string usgsGaugeId = snapshot.Site.UsgsGaugeId;

            var gauge = await this.EnsureGauge(snapshot.Site);

            if (gauge == null)
            {
                return;
            }

            await InactivateExistingGaugeReport(usgsGaugeId);

            var gaugeReport = new GaugeReport
            {
                AsOfUTC = DateTime.UtcNow,
                InstanceId = this.appInstanceId,
                Latest = true,
                Gauge = gauge
            };

            var gaugeValuesSameDate = snapshot
                .Values
                .Select(v => v.AsOf)
                .Distinct()
                .Count() == snapshot.Values.Count;

            if (gaugeValuesSameDate)
            {
                gaugeReport.AsOf = snapshot.Values.First().AsOf;
            }
            else
            {
                gaugeReport.AsOf = snapshot.Values.Max(v => v.AsOf);
                this.logger.LogInformation(
                    "{gauge}: Different as of dates for variables in same report (unexpected), using latest: {date}",
                    usgsGaugeId,
                    gaugeReport.AsOf);
            }

            foreach (var dataValue in snapshot.Values)
            {
                var gaugeValue = await this.RecordValue(dataValue, snapshot, gaugeReport);

                if (gaugeValue != null)
                {
                    gaugeReport.GaugeValueCount++;
                }
            }

            await this.riverDbContext.SaveChangesAsync();
        }

        private async Task InactivateExistingGaugeReport(string usgsGaugeId)
        {
            var existingLatestReport = await this.riverDbContext
                .GaugeReport
                .SingleOrDefaultAsync(r => r.UsgsGaugeId == usgsGaugeId && r.Latest);

            if (existingLatestReport != null)
            {
                existingLatestReport.Latest = false;
            }
        }

        private async Task<GaugeValue> RecordValue(
            DataValue value,
            RiverFlowSnapshotModel snapshot,
            GaugeReport report)
        {
            await this.EnsureVariable(value);
            return await this.AddGaugeValue(value, snapshot, report);
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

        private async Task<GaugeValue> AddGaugeValue(
            DataValue value,
            RiverFlowSnapshotModel snapshot,
            GaugeReport report)
        {
            var exists = await this.riverDbContext
                .GaugeValue
                .AnyAsync(gv => gv.AsOf == value.AsOf
                    && gv.UsgsGaugeId == snapshot.Site.UsgsGaugeId
                    && gv.Code == value.Code);

            if (!exists)
            {
                var gaugeValue = new GaugeValue
                {
                    AsOf = value.AsOf,
                    Code = value.Code,
                    Report = report,
                    UsgsGaugeId = snapshot.Site.UsgsGaugeId,
                    Value = value.Value
                };
                await this.riverDbContext.GaugeValue.AddAsync(gaugeValue);
                return gaugeValue;
            }

            return null;
        }

        private async Task<Gauge> EnsureGauge(SiteInfo siteInfo)
        {
            var gauge = await this.riverDbContext.Gauge
                .SingleOrDefaultAsync(g => g.UsgsGaugeId == siteInfo.UsgsGaugeId);

            if (gauge == null)
            {
                this.logger.LogWarning(
                    "Gauge {usgsGaugeId} not found in table. Either missing from data seeding or unknown queue guage id",
                    siteInfo.UsgsGaugeId);
                return null;
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

            return gauge;
        }
    }
}