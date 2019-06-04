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
        private readonly IStateRiverGaugeQuery stateRiverGaugeQuery;
        private readonly ILogger<StateController> logger;
        private readonly IStateFlowSummaryMapper mapper;

        public StateController(
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IStateRiverGaugeQuery stateRiverGaugeQuery,
            ILogger<StateController> logger,
            IStateFlowSummaryMapper mapper)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.stateRiverGaugeQuery = stateRiverGaugeQuery;
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

        [HttpGet("{state}/rivers")]
        public async Task<IActionResult> Rivers(string state)
        {
            var models = await this.stateRiverGaugeQuery.RunListAsync(state);
            return this.Ok(models);
        }
    }
}
