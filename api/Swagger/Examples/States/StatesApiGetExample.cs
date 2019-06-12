using System.Collections.Generic;
using RiverFlowApi.Data.Models.State;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Swagger.Examples.States
{
    public class StatesApiGetExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var stateModel = new StateModel
            {
                StateCode = "AL",
                Name = "Alabama",
                Region = "South",
                Division = "East South Central",
                GaugeCount = 18,
                Links = new List<Data.Models.HyperlinkModel>
                {
                    new Data.Models.HyperlinkModel(
                        href: "https://localhost:5001/states/AL",
                        rel: "states",
                        method: "GET")
                }
            };

            return new List<StateModel> { stateModel };
        }
    }
}