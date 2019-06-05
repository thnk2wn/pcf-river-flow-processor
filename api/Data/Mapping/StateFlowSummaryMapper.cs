using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RiverFlow.Common;
using RiverFlowApi.Data.DTO;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.Mapping
{
    public class StateFlowSummaryMapper : IStateFlowSummaryMapper
    {
        private readonly ILogger<StateFlowSummaryMapper> logger;

        public StateFlowSummaryMapper(ILogger<StateFlowSummaryMapper> logger)
        {
            this.logger = logger;
        }

        public List<RiverFlowStateSummaryModel> ToStateFlowModels(
            IEnumerable<StateFlowSummaryDTO> dtos)
        {
            var stateFlowModels = dtos
                .GroupBy(rg => rg.River.Name)
                .OrderBy(rg => rg.Key)
                .Select(grp => new RiverFlowStateSummaryModel
                {
                    River = grp.Key,

                    Gauges = grp
                        .GroupBy(g => g.Gauge.Id)
                        .Select(g => g.First())
                        .OrderBy(g => g.Gauge.Name)
                        .Select(item =>
                            new RiverFlowStateSummaryModel.GaugeModel
                            {
                                Name = item.Gauge.Name,
                                UsgsGaugeId = item.Gauge.Id,
                                LatestReading = new RiverFlowStateSummaryModel.GaugeReadingModel
                                {
                                    AsOf = GetAsOfDate(item.Value.AsOf, item.Gauge.TimeZoneAbbrev),
                                    AsOfUTC = item.Report.AsOfUTC,
                                    FlowCFS = grp.SingleOrDefault(_ =>
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00060")?.Value?.Value,
                                    HeightFeet = grp.SingleOrDefault(_ =>
                                        _.Gauge.Id == item.Gauge.Id &&
                                        _.Value.Code == "00065")?.Value?.Value,
                                    UsgsGaugeUrl = $"https://waterdata.usgs.gov/usa/nwis/uv?{item.Gauge.Id}"
                                }
                            }).ToList()
                }).ToList();
            return stateFlowModels;
        }

        private DateTime? GetAsOfDate(DateTime date, string timeZoneAbbrev)
        {
            var result = DateConversion.ForGaugeSite(date, timeZoneAbbrev);

            if (result.error != null)
            {
                this.logger.LogWarning(
                    "Error converting date '{date}': {reason}",
                    date,
                    result.error);
            }

            return result.date;
        }
    }
}