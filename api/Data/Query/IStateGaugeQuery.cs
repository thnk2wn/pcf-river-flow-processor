using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Query
{
    public interface IStateGaugeQuery
    {
        Task<List<StateGaugeModel>> RunListAsync(string state);
    }
}