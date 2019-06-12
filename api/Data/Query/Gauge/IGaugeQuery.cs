using System.Linq;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public interface IGaugeQuery
    {
        IQueryable<GaugeModel> Query(bool includeState);
    }
}