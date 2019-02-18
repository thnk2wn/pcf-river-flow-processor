using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using RiverFlowApi.Data.Entities;

namespace RiverFlowApi.Data.Seeding
{
    public class RiverLookupData
    {
        public RiverLookupData ReadAll()
        {
            this.States = this.GetRecords<State>();
            this.Rivers = this.GetRecords<River>();
            this.Gauges = this.GetRecords<Gauge>();
            this.RiverGauges = this.GetRecords<RiverGauge>();

             Console.WriteLine(
                 $"States: {this.States.Length}, " +
                 $"Rivers: {this.Rivers.Length}, " +
                 $"Gauges: {this.Gauges.Length}, " +
                 $"River Gauges: {this.RiverGauges.Length}");

            return this;
        }

        public State[] States { get; private set; }

        public River[] Rivers { get; private set; }

        public Gauge[] Gauges { get; private set; }

        public RiverGauge[] RiverGauges { get; private set; }

        private TEntity[] GetRecords<TEntity>()
        {
            var entity = typeof(TEntity).Name;
            var entityTypeInfo = typeof(TEntity).GetTypeInfo();
            var resource = $"{this.GetType().Namespace}.{entity}.csv";

            try
            {
                using (var resourceStream = entityTypeInfo.Assembly.GetManifestResourceStream(resource))
                using (var streamReader = new StreamReader(resourceStream))
                using (var csv = new CsvReader(streamReader))
                {
                    // navigation properties will cause issues, ignore any reference types
                    csv.Configuration.IgnoreReferences = true;
                    csv.Configuration.HeaderValidated = (isValid, headerNames, headerNameIndex, context) => {};
                    csv.Configuration.MissingFieldFound = (headerNames, index, context) => {};

                    var records = csv.GetRecords<TEntity>().ToArray();
                    return records;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting records for {entity} using {resource}", ex);
            }
        }
    }
}