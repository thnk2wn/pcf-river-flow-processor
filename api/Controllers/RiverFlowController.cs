using System;
using System.Collections.Generic;
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

        [HttpGet("{state}/summary")]
        public async Task<IActionResult> Get(string state)
        {
            var values = await this.GetFlow(state);

            return this.Ok(values);
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

        private async Task<object> GetFlow(string state)
        {
            var ctx = this.riverDbContext;

            // do we want rivers and gauges in state even if no flow data?
            // should we group in sql or get flat list from DB then group in memory?

            var riverGauges = await (
                from rg in ctx.RiverGauge
                join r in ctx.River on rg.RiverId equals r.RiverId
                where r.StateCode == state
                group rg by new {rg.RiverId, r.RiverSection}
                into grp
                select new {
                    RiverId = grp.Key.RiverId,
                    RiverName = grp.Key.RiverSection,
                    Gauges = grp
                        .Select(_ => _.UsgsGaugeId)
                        .ToList()
                }
            ).ToListAsync();

            var gaugeIds = riverGauges
                .SelectMany(rg => rg.Gauges)
                .ToList();

            // TODO: gauge values
            // TODO: Consider a GaugeReport type table that's 1:many with gauge and gaugevalue ties to it as well;
            //       this would simplify the fetch of the last gauge value as well as hold info that we checked even if no flow data

            return null;
        }
    }
}
