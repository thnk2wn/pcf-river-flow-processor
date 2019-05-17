using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("gauges")]
    [ApiController]
    public class GaugesController : ControllerBase
    {
        private readonly IFlowRecordingService flowRecordingService;

        public GaugesController(
            IFlowRecordingService flowRecordingService)
        {
            this.flowRecordingService = flowRecordingService;
        }

        [HttpPost]
        [Route("flow")]
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