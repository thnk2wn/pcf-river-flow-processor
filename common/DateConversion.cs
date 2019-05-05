using System;
using System.Collections.Generic;

namespace RiverFlow.Common
{
    public static class DateConversion
    {
        // Just the zones USGS will flow info on.
        // TODO: get distinct list after populating everything
        private static Dictionary<string, string> timezoneAbbrevMap = new Dictionary<string, string>
        {
            {"EST", "Eastern Standard Time"}
        };

        public static DateTime ForStorage(DateTimeOffset offset)
        {
            var dt = offset.ToUniversalTime();
            return dt.DateTime;
        }

        public static (DateTime? date, string error) ForGaugeSite(DateTime datetime, string timezoneAbbrev)
        {
            if (string.IsNullOrEmpty(timezoneAbbrev))
            {
                // shouldn't hit in practice but gauge timezone info isn't fetched until flow is fetched
                // shouldn't be any flow records w/o zone info but schema allows nulls, not sure if
                // service could ever
                return (null, "missing timezone abbreviation");
            }

            if (!timezoneAbbrevMap.ContainsKey(timezoneAbbrev))
            {
                return (null, $"Failed to find time zone abbrev '{timezoneAbbrev}'");
            }

            var timezoneName = timezoneAbbrevMap[timezoneAbbrev];
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
            var date = TimeZoneInfo.ConvertTimeFromUtc(datetime, tzi);
            return (date, null);
        }
    }
}