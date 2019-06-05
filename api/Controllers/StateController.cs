using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Query.State;
using RiverFlowApi.Swagger.Examples.States;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Controllers
{
    [Route("states")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateQuery stateQuery;

        public StateController(
            IStateQuery stateQuery)
        {
            this.stateQuery = stateQuery;
        }

        [HttpGet]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(StatesExample))]
        public async Task<IActionResult> List()
        {
            var states = await this.stateQuery.RunListAsync();
            return this.Ok(states);
        }
    }
}
