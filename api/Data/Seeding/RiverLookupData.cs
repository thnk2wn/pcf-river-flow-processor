using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using RiverFlowApi.Data.Entities;

namespace RiverFlowApi.Data.Seeding
{
    public static class RiverLookupData
    {
        public static TEntity[] GetRecords<TEntity>()
        {
            var entity = typeof(TEntity).Name;
            var entityTypeInfo = typeof(TEntity).GetTypeInfo();
            var resource = $"{typeof(RiverLookupData).Namespace}.{entity}.csv";

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