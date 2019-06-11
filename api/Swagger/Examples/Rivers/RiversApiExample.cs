using System.Collections.Generic;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Models.River;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Swagger.Examples.Rivers
{
    public class RiversApiExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<RiverModel>
            {
                new RiverModel
                {
                    RiverId = 151,
                    RiverSection = "Alameda Creek",
                    StateCode = "CA",
                    Region = "West",
                    Gauges = new List<RiverModel.GaugeModel>
                    {
                        new RiverModel.GaugeModel
                        {
                            Altitude = 85.65M,
                            Href = "/foo",
                            Lattitude = 37.58715679M,
                            Longitude = -121.960793M,
                            Zone = new SiteZoneInfo
                            {
                                DSTZoneAbbrev = "PDT",
                                DSTZoneOffset = "-07:00",
                                DefaultZoneAbbrev = "PST",
                                DefaultZoneOffset = "-08:00"
                            }
                        }
                    }
                }
            };
        }
    }
}