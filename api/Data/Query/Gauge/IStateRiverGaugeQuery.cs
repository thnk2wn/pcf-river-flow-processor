using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Query.Gauge
{
    public interface IStateRiverGaugeQuery
    {
        IQueryable<StateRiverGaugeModel> Query();
    }
}