using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("flow")]
    [ApiController]
    public class RiverFlowController : ControllerBase
    {
        private readonly IStateFlowSummaryQuery stateFlowSummaryQuery;
        private readonly IFlowRecordingService flowRecordingService;

        public RiverFlowController(
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IFlowRecordingService flowRecordingService)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.flowRecordingService = flowRecordingService;
        }

        [HttpGet("{state}/summary")]
        public async Task<IActionResult> Get(string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);

            var stateFlowModels = stateFlowDtos
                .GroupBy(rg => rg.River.Name)
                .OrderBy(rg => rg.Key)
                .Select(grp => new RiverFlowStateSummaryModel
                {
                    River = grp.Key,

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
                                    _.Gauge.Id == item.Gauge.Id &&
                                    _.Value.Code == "00060")?.Value?.Value,
                                HeightFeet = grp.SingleOrDefault(_ =>
                                    _.Gauge.Id == item.Gauge.Id &&
                                    _.Value.Code == "00065")?.Value?.Value
                            }).ToList()
                }).ToList();

            return this.Ok(stateFlowModels);
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
    }
}
