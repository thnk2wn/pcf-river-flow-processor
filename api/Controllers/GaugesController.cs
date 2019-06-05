using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Query.Gauge;
using RiverFlowApi.Data.Services;
using RiverFlowApi.Swagger.Examples.Gauges;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("gauges")]
    [ApiController]
    public class GaugesController : ControllerBase
    {
        private readonly IFlowRecordingService flowRecordingService;
        private readonly IStateFlowSummaryQuery stateFlowSummaryQuery;
        private readonly IStateFlowSummaryMapper stateFlowSummaryMapper;
        private readonly IStateRiverGaugeQuery stateRiverGaugeQuery;
        private readonly IStateGaugeQuery stateGaugeQuery;

        public GaugesController(
            IFlowRecordingService flowRecordingService,
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IStateFlowSummaryMapper stateFlowSummaryMapper,
            IStateRiverGaugeQuery stateRiverGaugeQuery,
            IStateGaugeQuery stateGaugeQuery)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.stateFlowSummaryMapper = stateFlowSummaryMapper;
            this.stateRiverGaugeQuery = stateRiverGaugeQuery;
            this.stateGaugeQuery = stateGaugeQuery;
            this.flowRecordingService = flowRecordingService;
        }

        // POST gauges/readings
        [HttpPost]
        [Route("readings")]
        public async Task Post(RiverFlowSnapshotModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await this.flowRecordingService.Record(model);
        }

        // GET gauges/state/readings/latest
        [HttpGet("{state}/readings/latest")]
        public async Task<IActionResult> GaugeLatestReadings(string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);
            var stateFlowModels = this.stateFlowSummaryMapper.ToStateFlowModels(stateFlowDtos);
            return this.Ok(stateFlowModels);
        }

        // GET gauges/state/rivers
        [HttpGet("{state}/rivers")]
        public async Task<IActionResult> GaugesViaRiver(string state)
        {
            var models = await this.stateRiverGaugeQuery.RunListAsync(state);
            return this.Ok(models);
        }

        /// <summary>
        /// Gets gauge information by state (excludes readings).
        /// </summary>
        /// <param name="state">State code to get gauge information for i.e. CA.</param>
        /// <returns>
        /// List of StageGaugeModel - top level gauge information (name, location, timezone etc.).
        /// </returns>
        // GET gauges/state
        [HttpGet("{state}")]
        [ProducesResponseType(typeof(List<StateGaugeModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GaugesByStateExample))]
        public async Task<IActionResult> Gauges(string state)
        {
            var models = await this.stateGaugeQuery.RunListAsync(state);
            return this.Ok(models);
        }
    }
}