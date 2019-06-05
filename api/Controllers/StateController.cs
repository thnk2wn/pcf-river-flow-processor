using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Query;

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
        public async Task<IActionResult> List()
        {
            var states = await this.stateQuery.RunListAsync();
            return this.Ok(states);
        }
    }
}
