using System.Collections.Generic;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Query.State
{
    public interface IStateQuery
    {
        Task<List<StateModel>> RunListAsync();
    }
}