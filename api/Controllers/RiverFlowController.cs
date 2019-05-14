using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("flow")]
    [ApiController]
    public class RiverFlowController : ControllerBase
    {
        private readonly IStateFlowSummaryQuery stateFlowSummaryQuery;
        private readonly IFlowRecordingService flowRecordingService;
        private readonly ILogger<RiverFlowController> logger;
        private readonly IStateFlowSummaryMapper mapper;

        public RiverFlowController(
            IStateFlowSummaryQuery stateFlowSummaryQuery,
            IFlowRecordingService flowRecordingService,
            ILogger<RiverFlowController> logger,
            IStateFlowSummaryMapper mapper)
        {
            this.stateFlowSummaryQuery = stateFlowSummaryQuery;
            this.flowRecordingService = flowRecordingService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("{state}/summary")]
        public async Task<IActionResult> Get(string state)
        {
            var stateFlowDtos = await this.stateFlowSummaryQuery.RunListAsync(state);
            var stateFlowModels = mapper.ToStateFlowModels(stateFlowDtos);
            return this.Ok(stateFlowModels);
        }

        [HttpPost]
        public async Task Post(RiverFlowSnapshotModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await this.flowRecordingService.Record(model);
        }
    }
}
