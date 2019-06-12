using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly IGaugeQuery gaugeQuery;

        public GaugesController(IGaugeQuery gaugeQuery)
        {
            this.gaugeQuery = gaugeQuery;
        }

        /// <summary>
        /// Gets gauge information by state (excludes readings).
        /// </summary>
        /// <param name="stateCode">Optional state code to get gauge information for i.e. CA.</param>
        /// <returns>
        /// List of GaugeModel - top level gauge information (name, location, timezone etc.).
        /// </returns>
        // GET gauges/state/{state}
        [HttpGet()]
        [ProducesResponseType(typeof(List<GaugeModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GaugesByStateExample))]
        public IActionResult List([FromQuery] string stateCode)
        {
            var query = this.gaugeQuery.Query();

            if (!string.IsNullOrEmpty(stateCode))
            {
                query = query.Where(q => q.State.StateCode == stateCode);
            }

            var gauges = query.ToList();

            return this.Ok(gauges);
        }
    }
}