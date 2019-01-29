using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data;

namespace RiverFlowApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RiverFlowController : ControllerBase
    {
        private readonly RiverDbContext riverDbContext;

        public RiverFlowController(RiverDbContext riverDbContext)
        {
            this.riverDbContext = riverDbContext;
        }

        // GET /riverflow
        [HttpGet]
        public async Task<string> Get()
        {
            var gauges = await this.riverDbContext.Gauge.ToListAsync();
            return $"Found {gauges.Count} gauges";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
