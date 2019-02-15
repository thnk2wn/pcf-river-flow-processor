using System.Threading.Tasks;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Services
{
    public interface IFlowRecordingService
    {
         Task Record(RiverFlowSnapshotModel model);
    }
}