using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;

namespace RiverFlowApi.Data.Query
{
    public interface IStateRiverGaugeQuery
    {
        Task<List<StateRiverGaugeDTO>> RunListAsync(string state);
    }
}