using System;
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
        /// <param name="region">Optional region to filter on i.e "West"</param>
        /// <param name="division">Optional division to filter by i.e. "South Atlantic".</param>
        /// <param name="stateCode">Optional state code to get gauge information for i.e. CA.</param>
        /// <param name="expand">
        /// Optional list of child resources to expand i.e. "state". State is auto-expanded when filtering on region or divsion.
        /// </param>
        /// <returns>
        /// List of GaugeModel - top level gauge information (name, location, timezone etc.).
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<GaugeModel>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GaugesApiGetExample))]
        public IActionResult List(
            [FromQuery] string region,
            [FromQuery] string division,
            [FromQuery] string stateCode,
            [FromQuery] List<string> expand)
        {
            expand = expand ?? new List<string>();
            bool includeState = expand
                .Any(e => string.Equals(e, "state", StringComparison.OrdinalIgnoreCase)) ||
                !string.IsNullOrEmpty(region) ||
                !string.IsNullOrEmpty(division);
            var query = this.gaugeQuery.Query(includeState);

            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(g => g.State.Region == region);
            }

            if (!string.IsNullOrEmpty(division))
            {
                query = query.Where(g => g.State.Division == division);
            }

            if (!string.IsNullOrEmpty(stateCode))
            {
                query = query.Where(g => g.State.StateCode == stateCode);
            }

            // TODO: orderby query string parameter
            query = query
                .OrderBy(g => g.State.StateCode)
                .ThenBy(g => g.Name);

            var gauges = query.ToList();

            return this.Ok(gauges);
        }

        /// <summary>
        /// Gets a single gauge by USGS gauge id.
        /// </summary>
        /// <param name="usgsGaugeId">USGS gauge id including any leading or trailing zeroes. i.e. 02177000.</param>
        /// <returns>Gauge model.</returns>
        [HttpGet("{usgsGaugeId}")]
        [ProducesResponseType(typeof(GaugeModel), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GaugeApiGetExample))]
        public IActionResult Get(string usgsGaugeId)
        {
            var query = this.gaugeQuery.Query(includeState: true);
            var gauge = query.FirstOrDefault(g => g.UsgsGaugeId == usgsGaugeId);
            return this.Ok(gauge);
        }
    }
}