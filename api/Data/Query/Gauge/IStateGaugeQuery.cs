using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public interface IStateGaugeQuery
    {
        Task<List<StateGaugeModel>> RunListAsync(string state);
    }
}