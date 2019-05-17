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
        private readonly IStateFlowSummaryQuery stateFlowSummaryQuery;
        private readonly ILogger<StateController> logger;
        private readonly IStateFlowSummaryMapper mapper;

        public StateController(
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            ILogger<StateController> logger,
            IStateFlowSummaryMapper mapper)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("{state}/flow")]
        public async Task<IActionResult> Flow(string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);
            var stateFlowModels = mapper.ToStateFlowModels(stateFlowDtos);
            return this.Ok(stateFlowModels);
        }
    }
}
