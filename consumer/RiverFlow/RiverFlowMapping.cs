using System;
using System.Linq;
using RiverFlowProcessor.USGS;
using TimeZone = RiverFlowProcessor.USGS.TimeZone;

namespace RiverFlowProcessor.RiverFlow
{
    public static class RiverFlowMapping
    {
        public static RiverFlowSnapshot MapFlowData(string usgsGaugeId, StreamFlow usgsStreamFlow)
        {
            // Current assumptions for simplicity
            // 1) One site at a time though API supports multiple
            // 2) One parameter value at a time (most recent) though API supports time period

            var timeSeries = usgsStreamFlow.Value.GetTimeSeriesForSite(usgsGaugeId);

            var snapshot = new RiverFlowSnapshot
            {
                AsOfUTC = DateTime.UtcNow,
                Site = GetSiteInfo(timeSeries.FirstOrDefault()?.SourceInfo)
            };

            foreach (var ts in timeSeries)
            {
                var dataValue = GetDataValue(ts);

                if (dataValue != null)
                {
                    snapshot.Values.Add(dataValue);
                }
            }

            if (!snapshot.Values.Any())
            {
                return null;
            }

            return snapshot;
        }

        private static RiverFlowSnapshot.DataValue GetDataValue(TimeSery ts)
        {
            if (ts == null || ts.Values?.Length != 1 || ts.Values[0].Value?.Length != 1)
            {
                return null;
            }

            var tsValue = ts.Values[0].Value[0];
            var parsed = double.TryParse(tsValue.Value, out double value);

            if (!parsed || ts.Variable.NoDataValue == (long)value)
            {
                return null;
            }

            var dataValue = new RiverFlowSnapshot.DataValue
            {
                AsOf = tsValue.DateTime,
                Code = ts.Variable.VariableCode[0].Value,
                Decription = ts.Variable.VariableDescription,
                Name = System.Net.WebUtility.HtmlDecode(ts.Variable.VariableName),
                Unit = ts.Variable.Unit.UnitCode,
                Value = value
            };
            return dataValue;
        }

        private static RiverFlowSnapshot.SiteInfo GetSiteInfo(SourceInfo sourceInfo)
        {
            if (sourceInfo == null)
            {
                return null;
            }

            var time = sourceInfo.TimeZoneInfo;
            var site = new RiverFlowSnapshot.SiteInfo
            {
                UsgsGaugeId = sourceInfo.SiteCode[0].Value,
                DefaultTimeZone = GetTimezone(time.DefaultTimeZone),
                DaylightSavingsTimeZone = GetTimezone(time.DaylightSavingsTimeZone),
                UsesDaylightSavingsTime = time.SiteUsesDaylightSavingsTime
            };
            return site;
        }

        private static RiverFlowSnapshot.SiteTimeZone GetTimezone(TimeZone tz)
        {
            return new RiverFlowSnapshot.SiteTimeZone
            {
                ZoneAbbreviation = tz.ZoneAbbreviation,
                ZoneOffset = tz.ZoneOffset
            };
        }
    }
}