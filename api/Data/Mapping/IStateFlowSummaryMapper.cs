using System.Collections.Generic;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Mapping
{
    public interface IStateFlowSummaryMapper
    {
         List<RiverFlowStateSummaryModel> ToStateFlowModels(IEnumerable<StateFlowSummaryDTO> dtos);
    }
}