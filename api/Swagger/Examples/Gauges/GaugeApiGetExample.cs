using System.Collections.Generic;
using RiverFlowApi.Data.Models.Gauge;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Swagger.Examples.Gauges
{
    public class GaugeApiGetExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new GaugeModel
            {
                UsgsGaugeId = "01010070",
                Name = "Big Black River near Depot Mtn, Maine",
                Lattitude = 46.89388889M,
                Longitude = -69.7516667M,
                Altitude = 885.0M,
                Zone = new SiteZoneInfo
                {
                    DSTZoneAbbrev = "EDT",
                    DSTZoneOffset = "-04:00",
                    DefaultZoneAbbrev = "EST",
                    DefaultZoneOffset = "-05:00",
                    ZoneUsesDST = true
                },
                State = new Data.Models.State.StateModel
                {
                    StateCode = "ME",
                    Links = new List<Data.Models.HyperlinkModel>
                    {
                        new Data.Models.HyperlinkModel(
                            href: "https://localhost:5001/states/ME",
                            rel: "states",
                            method: "GET")
                    }
                }
            };
        }
    }
}