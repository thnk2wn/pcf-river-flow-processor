using System.Linq;
using RiverFlowApi.Data.Models.River;

namespace RiverFlowApi.Data.Query.River
{
    public interface IRiverQuery
    {
        IQueryable<RiverModel> Query(bool includeGauges);
    }
}