using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;

namespace RiverFlowApi.Data.Import
{
    public class RiverImportDataReader
    {
        public RiverImportDataReader ReadAll()
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

        private T[] GetRecords<T>()
        {
            var entity = typeof(T).Name;
            var typeInfo = typeof(T).GetTypeInfo();
            var resource = $"{typeInfo.Namespace}.ImportFiles.{entity}.csv";

            try
            {
                using (var resourceStream = typeInfo.Assembly.GetManifestResourceStream(resource))
                using (var streamReader = new StreamReader(resourceStream))
                using (var csv = new CsvReader(streamReader))
                {
                    // navigation properties will cause issues, ignore any reference types
                    csv.Configuration.IgnoreReferences = true;
                    var records = csv.GetRecords<T>().ToArray();
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