using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Entities;

namespace RiverFlowApi.Data.Query
{
    public class StateFlowSummaryQuery
        : ParameterizedQuery<StateFlowSummaryDTO, string>, IStateFlowSummaryQuery
    {
        private readonly RiverDbContext riverDbContext;

        public StateFlowSummaryQuery(RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
        }

        protected override async Task<IEnumerable<StateFlowSummaryDTO>> QueryAsync(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var ctx = this.riverDbContext;

            var rawFlowData = await (
                from gaugeReport in ctx.GaugeReport
                join gaugeValue in ctx.GaugeValue on gaugeReport.ReportId equals gaugeValue.ReportId
                join variable in ctx.Variable on gaugeValue.Code equals variable.Code
                join gauge in ctx.Gauge on gaugeValue.UsgsGaugeId equals gauge.UsgsGaugeId
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
                where river.StateCode == state && gaugeReport.Latest
                select new StateFlowSummaryDTO
                {
                    River = new StateFlowSummaryDTO.RiverDTO
                    {
                        Id = river.RiverId,
                        Name = river.RiverSection
                    },
                    Gauge = new StateFlowSummaryDTO.GaugeDTO
                    {
                        Id = gauge.UsgsGaugeId,
                        Name = gauge.Name,
                        TimeZoneAbbrev = gauge.DefaultZoneAbbrev
                    },
                    Value = new StateFlowSummaryDTO.ValueDTO
                    {
                        AsOf = gaugeValue.AsOf,
                        Code = variable.Code,
                        Name = variable.Name,
                        Unit = variable.Unit,
                        Value = gaugeValue.Value
                    },
                    Report = new StateFlowSummaryDTO.ReportDTO
                    {
                        AsOf = gaugeReport.AsOf,
                        AsOfUTC = gaugeReport.AsOfUTC
                    }
                }
            ).ToListAsync();

            return rawFlowData;
        }
    }
}