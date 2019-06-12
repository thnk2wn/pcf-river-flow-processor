using System.Linq;
using RiverFlowApi.Data.Models.State;

namespace RiverFlowApi.Data.Query.State
{
    public interface IStateQuery
    {
        IQueryable<StateModel> Query();
    }
}