using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Query.Gauge;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("gauge-reports")]
    [ApiController]
    public class GaugeReportsController : ControllerBase
    {
        private readonly IFlowRecordingService flowRecordingService;
        private readonly IStateFlowSummaryQuery stateFlowSummaryQuery;
        private readonly IStateFlowSummaryMapper stateFlowSummaryMapper;

        public GaugeReportsController(
            IFlowRecordingService flowRecordingService,
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IStateFlowSummaryMapper stateFlowSummaryMapper)
        {
            this.flowRecordingService = flowRecordingService;
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.stateFlowSummaryMapper = stateFlowSummaryMapper;
        }

        // POST gauge-reports
        [HttpPost]
        public async Task AddGaugeReport(RiverFlowSnapshotModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await this.flowRecordingService.Record(model);
        }

        // TODO: One gauge report query endpoint with query string parameters for state, region, gaugeId etc.?

        // GET gauge-reports/latest
        [HttpGet("latest")]
        public async Task<IActionResult> GaugeLatestReadings([FromQuery] string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);
            var stateFlowModels = this.stateFlowSummaryMapper.ToStateFlowModels(stateFlowDtos);
            return this.Ok(stateFlowModels);
        }
    }
}