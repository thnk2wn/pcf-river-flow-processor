using System;
using Microsoft.Extensions.Configuration;

namespace RiverFlowApi.Configuration
{
    public static class IConfigurationExtensions
    {
        public static string GetAppInstanceId(this IConfiguration config)
        {
            return $"{config.InstanceId()}:{config.InstanceIndex()}";
        }

        private static string InstanceId(this IConfiguration config)
        {
            return config["CF_INSTANCE_GUID"] ?? Guid.Empty.ToString();
        }

        private static string InstanceIndex(this IConfiguration config)
        {
            return config["CF_INSTANCE_INDEX"] ?? "0";
        }
    }
}