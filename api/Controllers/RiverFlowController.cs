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

        [HttpGet("{state}")]
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
            var query = this.riverDbContext.GaugeValue.AsQueryable();

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(gv => gv.Gauge.StateCode == state);
            }

            var results = await query.Join(this.riverDbContext.Gauge,
                gv => gv.UsgsGaugeId,
                g => g.UsgsGaugeId,
                (gv, g) => new
                {
                    GaugeId = g.UsgsGaugeId,
                    GaugeName = g.Name,
                    Variable = gv.Code,
                    Value = gv.Value,
                    VariableName = gv.Variable.Name,
                    VariableDesc = gv.Variable.Description,
                    VariableUnit = gv.Variable.Unit
                })
                .ToListAsync();
            return results;
        }
    }
}
