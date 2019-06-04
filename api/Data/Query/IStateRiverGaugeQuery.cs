using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Query
{
    public interface IStateRiverGaugeQuery
    {
        Task<List<StateRiverGaugeModel>> RunListAsync(string state);
    }
}