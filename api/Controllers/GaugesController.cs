using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Query.Gauge;
using RiverFlowApi.Swagger.Examples.Gauges;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("gauges")]
    [ApiController]
    public class GaugesController : ControllerBase
    {
        private readonly IStateRiverGaugeQuery stateRiverGaugeQuery;
        private readonly IStateGaugeQuery stateGaugeQuery;

        public GaugesController(
            IStateRiverGaugeQuery stateRiverGaugeQuery,
            IStateGaugeQuery stateGaugeQuery)
        {
            this.stateRiverGaugeQuery = stateRiverGaugeQuery;
            this.stateGaugeQuery = stateGaugeQuery;
        }

        // TODO: One gauge query endpoint with query string parameters for state, region etc.?

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
        // GET gauges/state/{state}
        [HttpGet("state/{state}")]
        [ProducesResponseType(typeof(List<StateGaugeModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GaugesByStateExample))]
        public async Task<IActionResult> Gauges(string state)
        {
            var models = await this.stateGaugeQuery.RunListAsync(state);
            return this.Ok(models);
        }
    }
}