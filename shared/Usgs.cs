
namespace RiverFlow.Shared
{
    public static class Usgs
    {
        public static string FormatGaugeId(string gaugeId)
        {
            // Must be at least 8 characters. Leading zeroes removed with some export/imports. Max 15.
            return gaugeId.PadLeft(8, '0');
        }
    }
}