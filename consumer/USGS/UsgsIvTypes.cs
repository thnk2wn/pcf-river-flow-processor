// Initially auto-generated from https://quicktype.io/ with sample data from https://waterservices.usgs.gov/nwis/iv/?sites=03539600&format=json
// 
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using RiverFlowProcessor.USGS;
//
//    var streamFlow = StreamFlow.FromJson(jsonString);

namespace RiverFlowProcessor.USGS
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class StreamFlow
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("declaredType")]
        public string DeclaredType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("value")]
        public StreamFlowValue Value { get; set; }

        [JsonProperty("nil")]
        public bool Nil { get; set; }

        [JsonProperty("globalScope")]
        public bool GlobalScope { get; set; }

        [JsonProperty("typeSubstituted")]
        public bool TypeSubstituted { get; set; }

        public TimeSery GetTimeSeries(string variableCode)
        {
            if (this.Value == null || this.Value.TimeSeries == null) 
            {
                return null;
            }
            
            var timeSeries = this.Value?.TimeSeries?
                .SingleOrDefault(ts => 
                    ts.Variable.VariableCode.Any(
                        vc => vc.Value == variableCode));
            return timeSeries;
        }

        public TimeSeriesValue GetLastTimeSeriesValue(string variableCode) 
        {
            var timeSeries = GetTimeSeries(variableCode);

            if (timeSeries == null || timeSeries.Values == null) 
            {
                return null;
            }

            var timeSeriesValue = timeSeries.Values.FirstOrDefault()?.Value?.LastOrDefault();
            return timeSeriesValue;
        }

        public SourceInfo GetSource() 
        {
            var source = this.Value?.TimeSeries?.FirstOrDefault(ts => ts.SourceInfo != null)?.SourceInfo;
            return source;
        }
    }

    public partial class StreamFlowValue
    {
        [JsonProperty("queryInfo")]
        public QueryInfo QueryInfo { get; set; }

        [JsonProperty("timeSeries")]
        public TimeSery[] TimeSeries { get; set; }
    }

    public partial class QueryInfo
    {
        [JsonProperty("queryURL")]
        public Uri QueryUrl { get; set; }

        [JsonProperty("criteria")]
        public Criteria Criteria { get; set; }

        [JsonProperty("note")]
        public Note[] Note { get; set; }
    }

    public partial class Criteria
    {
        [JsonProperty("locationParam")]
        public string LocationParam { get; set; }

        [JsonProperty("variableParam")]
        public string VariableParam { get; set; }

        [JsonProperty("parameter")]
        public object[] Parameter { get; set; }
    }

    public partial class Note
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class TimeSery
    {
        [JsonProperty("sourceInfo")]
        public SourceInfo SourceInfo { get; set; }

        [JsonProperty("variable")]
        public Variable Variable { get; set; }

        [JsonProperty("values")]
        public TimeSeryValue[] Values { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class SourceInfo
    {
        [JsonProperty("siteName")]
        public string SiteName { get; set; }

        [JsonProperty("siteCode")]
        public SiteCode[] SiteCode { get; set; }

        [JsonProperty("timeZoneInfo")]
        public TimeZoneInfo TimeZoneInfo { get; set; }

        [JsonProperty("geoLocation")]
        public GeoLocation GeoLocation { get; set; }

        [JsonProperty("note")]
        public object[] Note { get; set; }

        [JsonProperty("siteType")]
        public object[] SiteType { get; set; }

        [JsonProperty("siteProperty")]
        public SiteProperty[] SiteProperty { get; set; }
    }

    public partial class GeoLocation
    {
        [JsonProperty("geogLocation")]
        public GeogLocation GeogLocation { get; set; }

        [JsonProperty("localSiteXY")]
        public object[] LocalSiteXy { get; set; }
    }

    public partial class GeogLocation
    {
        [JsonProperty("srs")]
        public string Srs { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }

    public partial class SiteCode
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("agencyCode")]
        public string AgencyCode { get; set; }
    }

    public partial class SiteProperty
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }
    }

    public partial class TimeZoneInfo
    {
        [JsonProperty("defaultTimeZone")]
        public TimeZone DefaultTimeZone { get; set; }

        [JsonProperty("daylightSavingsTimeZone")]
        public TimeZone DaylightSavingsTimeZone { get; set; }

        [JsonProperty("siteUsesDaylightSavingsTime")]
        public bool SiteUsesDaylightSavingsTime { get; set; }
    }

    public partial class TimeZone
    {
        [JsonProperty("zoneOffset")]
        public string ZoneOffset { get; set; }

        [JsonProperty("zoneAbbreviation")]
        public string ZoneAbbreviation { get; set; }
    }

    public partial class TimeSeryValue
    {
        [JsonProperty("value")]
        public TimeSeriesValue[] Value { get; set; }

        [JsonProperty("qualifier")]
        public Qualifier[] Qualifier { get; set; }

        [JsonProperty("qualityControlLevel")]
        public object[] QualityControlLevel { get; set; }

        [JsonProperty("method")]
        public Method[] Method { get; set; }

        [JsonProperty("source")]
        public object[] Source { get; set; }

        [JsonProperty("offset")]
        public object[] Offset { get; set; }

        [JsonProperty("sample")]
        public object[] Sample { get; set; }

        [JsonProperty("censorCode")]
        public object[] CensorCode { get; set; }
    }

    public partial class Method
    {
        [JsonProperty("methodDescription")]
        public string MethodDescription { get; set; }

        [JsonProperty("methodID")]
        public long MethodId { get; set; }
    }

    public partial class Qualifier
    {
        [JsonProperty("qualifierCode")]
        public string QualifierCode { get; set; }

        [JsonProperty("qualifierDescription")]
        public string QualifierDescription { get; set; }

        [JsonProperty("qualifierID")]
        public long QualifierId { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("vocabulary")]
        public string Vocabulary { get; set; }
    }

    public partial class TimeSeriesValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("qualifiers")]
        public string[] Qualifiers { get; set; }

        [JsonProperty("dateTime")]
        public DateTimeOffset DateTime { get; set; }
    }

    public partial class Variable
    {
        [JsonProperty("variableCode")]
        public VariableCode[] VariableCode { get; set; }

        [JsonProperty("variableName")]
        public string VariableName { get; set; }

        [JsonProperty("variableDescription")]
        public string VariableDescription { get; set; }

        [JsonProperty("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("unit")]
        public Unit Unit { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }

        [JsonProperty("note")]
        public object[] Note { get; set; }

        [JsonProperty("noDataValue")]
        public long NoDataValue { get; set; }

        [JsonProperty("variableProperty")]
        public object[] VariableProperty { get; set; }

        [JsonProperty("oid")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Oid { get; set; }
    }

    public partial class Options
    {
        [JsonProperty("option")]
        public Option[] Option { get; set; }
    }

    public partial class Option
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("optionCode")]
        public string OptionCode { get; set; }
    }

    public partial class Unit
    {
        [JsonProperty("unitCode")]
        public string UnitCode { get; set; }
    }

    public partial class VariableCode
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("vocabulary")]
        public string Vocabulary { get; set; }

        [JsonProperty("variableID")]
        public long VariableId { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }
    }

    public enum Name { CountyCd, HucCd, SiteTypeCd, StateCd };

    public partial class StreamFlow
    {
        public static StreamFlow FromJson(string json) => JsonConvert.DeserializeObject<StreamFlow>(json, RiverFlowProcessor.USGS.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this StreamFlow self) => JsonConvert.SerializeObject(self, RiverFlowProcessor.USGS.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                NameConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "countyCd":
                    return Name.CountyCd;
                case "hucCd":
                    return Name.HucCd;
                case "siteTypeCd":
                    return Name.SiteTypeCd;
                case "stateCd":
                    return Name.StateCd;
            }
            throw new Exception("Cannot unmarshal type Name");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Name)untypedValue;
            switch (value)
            {
                case Name.CountyCd:
                    serializer.Serialize(writer, "countyCd");
                    return;
                case Name.HucCd:
                    serializer.Serialize(writer, "hucCd");
                    return;
                case Name.SiteTypeCd:
                    serializer.Serialize(writer, "siteTypeCd");
                    return;
                case Name.StateCd:
                    serializer.Serialize(writer, "stateCd");
                    return;
            }
            throw new Exception("Cannot marshal type Name");
        }

        public static readonly NameConverter Singleton = new NameConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
