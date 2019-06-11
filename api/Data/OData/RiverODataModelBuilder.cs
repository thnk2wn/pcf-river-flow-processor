using System;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using RiverFlowApi.Data.Models.Gauge;

namespace RiverFlowApi.Data.OData
{
    public class RiverOdataModelBuilder
    {
        public IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);

            builder.EntitySet<StateRiverGaugeModel>(nameof(StateRiverGaugeModel))
                .EntityType
                .HasKey(m => m.RiverId)
                .Filter()
                .Count()
                .Expand()
                .OrderBy()
                .Page()
                .Select();

            // builder.ComplexType<StateRiverGaugeModel>()

            return builder.GetEdmModel();
        }
    }
}
