using System;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RiverFlow.Common;
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
        private readonly ILogger<RiverFlowController> logger;

        public RiverFlowController(
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IFlowRecordingService flowRecordingService,
            ILogger<RiverFlowController> logger)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.flowRecordingService = flowRecordingService;
            this.logger = logger;
        }

        [HttpGet("{state}/summary")]
        public async Task<IActionResult> Get(string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);

            // TODO: consider moving below to separate mapping file
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
                                LatestReading = new RiverFlowStateSummaryModel.GaugeReadingModel
                                {
                                    AsOf = GetAsOfDate(item.Value.AsOf, item.Gauge.TimeZoneAbbrev),
                                    AsOfUTC = item.Report.AsOfUTC,
                                    FlowCFS = grp.SingleOrDefault(_ =>
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00060")?.Value?.Value,
                                    HeightFeet = grp.SingleOrDefault(_ =>
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00065")?.Value?.Value,
                                    UsgsGaugeUrl = $"https://waterdata.usgs.gov/usa/nwis/uv?{item.Gauge.Id}"
                                }
                            }).ToList()
                }).ToList();

            return this.Ok(stateFlowModels);
        }

        private DateTime? GetAsOfDate(DateTime date, string timeZoneAbbrev)
        {
            var result = DateConversion.ForGaugeSite(date, timeZoneAbbrev);

            if (result.error != null)
            {
                this.logger.LogWarning(
                    "Error converting date '{date}': {reason}",
                    date,
                    result.error);
            }

            return result.date;
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
