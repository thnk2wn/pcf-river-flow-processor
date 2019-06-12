using System.Collections.Generic;
using RiverFlowApi.Data.Models;
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
                    State = new Data.Models.State.StateModel
                    {
                        StateCode = "CA",
                        Links = new List<Data.Models.HyperlinkModel>
                        {
                            new Data.Models.HyperlinkModel(
                                href: "https://localhost:5001/states/CA",
                                rel: "states",
                                method: "GET")
                        }
                    },
                    Region = "West",
                    Gauges = new List<GaugeModel>
                    {
                        new GaugeModel
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