using System.Collections.Generic;
using RiverFlowApi.Data.Models.Gauge;
using RiverFlowApi.Data.Models.Hypermedia;
using RiverFlowApi.Data.Models.River;
using RiverFlowApi.Data.Models.State;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Swagger.Examples.Rivers
{
    public class RiverApiExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RiverModel
            {
                RiverId = 151,
                RiverSection = "Alameda Creek",
                State = new StateModel
                {
                    StateCode = "ME",
                    Links = new List<HyperlinkModel>
                    {
                        new HyperlinkModel(
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
            };
        }
    }
}