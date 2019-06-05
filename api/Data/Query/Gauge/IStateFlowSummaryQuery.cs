using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;

namespace RiverFlowApi.Data.Query.Gauge
{
    public interface IStateFlowSummaryQuery
    {
        Task<List<StateFlowSummaryDTO>> RunListAsync(string state);
    }
}