using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("flow")]
    [ApiController]
    public class RiverFlowController : ControllerBase
    {
        private readonly RiverDbContext riverDbContext;
        private readonly IFlowRecordingService flowRecordingService;

        public RiverFlowController(
            RiverDbContext riverDbContext,
            IFlowRecordingService flowRecordingService)
        {
            this.flowRecordingService = flowRecordingService;
            this.riverDbContext = riverDbContext;
        }

        [HttpGet("{state}/summary")]
        public async Task<IActionResult> Get(string state)
        {
            var values = await this.GetFlow(state);

            return this.Ok(values);
        }

        [HttpPost]
        public async Task Post(RiverFlowSnapshotModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await this.flowRecordingService.Record(model);
        }

        private async Task<object> GetFlow(string state)
        {
            // TODO: move this method to a query type class
            var ctx = this.riverDbContext;
            var sw = Stopwatch.StartNew();

            var rawFlowData = await (
                from gaugeValue in ctx.GaugeValue
                join variable in ctx.Variable on gaugeValue.Code equals variable.Code
                join gauge in ctx.Gauge on gaugeValue.UsgsGaugeId equals gauge.UsgsGaugeId
                join gaugeReport in ctx.GaugeReport on gauge.UsgsGaugeId equals gaugeReport.UsgsGaugeId
                join riverGauge in ctx.RiverGauge on gauge.UsgsGaugeId equals riverGauge.UsgsGaugeId
                join river in ctx.River on riverGauge.RiverId equals river.RiverId
                where river.StateCode == state && gaugeReport.Latest
                select new
                {
                    River = new
                    {
                        Id = river.RiverId,
                        Name = river.RiverSection
                    },
                    Gauge = new
                    {
                        Id = gauge.UsgsGaugeId,
                        Name = gauge.Name,
                    },
                    Value = new
                    {
                        Code = variable.Code,
                        Name = variable.Name,
                        Unit = variable.Unit,
                        Value = gaugeValue.Value
                    }
                }
            ).ToListAsync();
            sw.Stop();

            var model = new RiverFlowStateSummaryModel
            {
                Rivers = rawFlowData
                    .GroupBy(rg => rg.River)
                    .OrderBy(rg => rg.Key.Name)
                    .Select(grp => new RiverFlowStateSummaryModel.RiversModel
                    {
                        River = grp.Key.Name,

                        Gauges = grp
                            .GroupBy(g => g.Gauge.Id)
                            .Select(g => g.First())
                            .OrderBy(g => g.Gauge.Name)
                            .Select(item =>
                                new RiverFlowStateSummaryModel.GaugeModel
                                {
                                    Name = item.Gauge.Name,
                                    UsgsGaugeId = item.Gauge.Id,
                                    FlowCFS = grp.SingleOrDefault(_ =>
                                        _.River.Id == grp.Key.Id &&
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00060")?.Value?.Value,
                                    HeightFeet = grp.SingleOrDefault(_ =>
                                        _.River.Id == grp.Key.Id &&
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00065")?.Value?.Value
                                }).ToList()
                    }).ToList()
            };

            return model;
        }
    }
}
