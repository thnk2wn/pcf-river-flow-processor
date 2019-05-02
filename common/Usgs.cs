
using System;

namespace RiverFlow.Common
{
    public static class Usgs
    {
        public static string FormatGaugeId(string gaugeId)
        {
            if (gaugeId == null)
            {
                throw new ArgumentNullException(nameof(gaugeId));
            }

            if (gaugeId == string.Empty)
            {
                throw new ArgumentException("gaugeId must be set", nameof(gaugeId));
            }

            // Must be at least 8 characters. Leading zeroes removed with some export/imports. Max 15.
            return gaugeId.PadLeft(8, '0');
        }
    }
}