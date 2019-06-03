using System.Collections.Generic;
using System.Linq;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;

namespace RiverFlowApi.Data.Mapping
{
    public static class StateRiverGaugeMapping
    {
        public static List<StateRiverGaugeModel> ToModels(this IEnumerable<StateRiverGaugeDTO> dtos)
        {
            var models = dtos
                .GroupBy(dto => dto.RiverId)
                .Select(grp => new StateRiverGaugeModel
                {
                    RiverId = grp.Key,
                    RiverSection = grp.First().RiverSection,
                    Gauges = grp.Select(g => new StateRiverGaugeModel.GaugeModel
                    {
                        Altitude = g.Altitude,
                        Lattitude = g.Lattitude,
                        Longitude = g.Longitude,
                        Name = g.Name,
                        UsgsGaugeId = g.UsgsGaugeId
                    })
                    .OrderBy(g => g.Name)
                    .ThenBy(g => g.UsgsGaugeId)
                    .ToList()
                })
                .ToList();
            return models;
        }
    }
}