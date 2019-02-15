using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Services;

namespace RiverFlowApi.Controllers
{
    [Route("flow")]
    [ApiController]
    public class RiverFlowController : ControllerBase
    {
        private readonly RiverDbContext riverDbContext;
        private readonly IFlowRecordingService flowRecordingService;

        public RiverFlowController(
            RiverDbContext riverDbContext,
            IFlowRecordingService flowRecordingService)
        {
            this.flowRecordingService = flowRecordingService;
            this.riverDbContext = riverDbContext;
        }

        [HttpGet("{state}")]
        public async Task<string> Get(string state)
        {
            var values = await this.riverDbContext
                .GaugeValue
                .Where(gv => gv.Gauge.StateCode == state)
                .ToListAsync();

            return $"Found {values.Count} gauge values";
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
