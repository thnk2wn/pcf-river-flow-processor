using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Query.Gauge;
using RiverFlowApi.Swagger.Examples.Gauges;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("gauges")]
    [ApiController]
    public class GaugesController : ODataController
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
        [EnableQuery]
        [Produces(MediaTypeNames.Application.Json)]
        public IQueryable<StateRiverGaugeModel> GaugesViaRiver()
        {
            var queryableResult = this.stateRiverGaugeQuery.Query();
            return queryableResult;
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