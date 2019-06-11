using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Models.River;
using RiverFlowApi.Data.Query.River;
using RiverFlowApi.Swagger.Examples.Rivers;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("rivers")]
    public class RiversController : ControllerBase
    {
        private readonly IRiverQuery riverQuery;

        public RiversController(IRiverQuery riverQuery)
        {
            this.riverQuery = riverQuery;
        }

        /// <summary>
        /// Gets river information, optionally including gauges.
        /// </summary>
        /// <param name="region">Optional region to filter on i.e "West"</param>
        /// <param name="division">Optional division to filter by i.e. "South Atlantic".</param>
        /// <param name="stateCode">Optional state code to filter by i.e. "TN".</param>
        /// <param name="expand">List of child resources to expand i.e. "gauges".</param>
        /// <returns>List of RiverModel.</returns>
        [HttpGet]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(RiversApiExample))]
        [ProducesResponseType(typeof(List<RiverModel>), (int)HttpStatusCode.OK)]
        public IActionResult List(
            [FromQuery] string region,
            [FromQuery] string division,
            [FromQuery] string stateCode,
            [FromQuery] List<string> expand)
        {
            expand = expand ?? new List<string>();
            bool includeGauges = expand
                .Any(e => string.Equals(e, "gauges", StringComparison.OrdinalIgnoreCase));
            var query = this.riverQuery.Query(includeGauges);

            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(q => q.Region == region);
            }

            if (!string.IsNullOrEmpty(division))
            {
                query = query.Where(q => q.Division == division);
            }

            if (!string.IsNullOrEmpty(stateCode))
            {
                query = query.Where(q => q.StateCode == stateCode);
            }

            var models = query.ToList();
            return this.Ok(models);
        }
    }
}