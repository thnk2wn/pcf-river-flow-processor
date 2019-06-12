using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Query.State;
using RiverFlowApi.Swagger.Examples.States;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("states")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly IStateQuery stateQuery;

        public StatesController(
            IStateQuery stateQuery)
        {
            this.stateQuery = stateQuery;
        }

        /// <summary>
        /// Gets a list of states including region and division names, gauge count.
        /// </summary>
        /// <returns>List of state models.</returns>
        [HttpGet]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(StatesApiGetExample))]
        public IActionResult List()
        {
            var states = this.stateQuery
                .Query()
                .OrderBy(s => s.Name)
                .ToList();

            return this.Ok(states);
        }

        /// <summary>
        /// Gets a single state by state code (i.e. CA) including region and division names, gauge count.
        /// </summary>
        /// <param name="stateCode">State code to get state info by.</param>
        /// <returns>State model.</returns>
        [HttpGet("{stateCode}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(StateApiGetExample))]
        public IActionResult Get(string stateCode)
        {
            var state = this.stateQuery
                .Query()
                .Where(s => s.StateCode == stateCode)
                .FirstOrDefault();
            return this.Ok(state);
        }
    }
}
