using System.Collections.Generic;
using RiverFlowApi.Data.Models.State;
using Swashbuckle.AspNetCore.Filters;

namespace RiverFlowApi.Swagger.Examples.States
{
    public class StatesExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<StateModel>
            {
                {
                    new StateModel
                    {
                        StateCode = "AL",
                        Name = "Alabama",
                        Region = "South",
                        Division = "East South Central",
                        GaugeCount = 18
                    }
                }
            };
        }
    }
}