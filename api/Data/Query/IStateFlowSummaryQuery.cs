using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;

namespace RiverFlowApi.Data.Query
{
    public interface IStateFlowSummaryQuery
    {
        Task<List<StateFlowSummaryDTO>> RunListAsync(string state);
    }
}