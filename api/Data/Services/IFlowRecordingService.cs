using System.Threading.Tasks;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Services
{
    public interface IFlowRecordingService
    {
         Task Record(RiverFlowSnapshotModel model);
    }
}