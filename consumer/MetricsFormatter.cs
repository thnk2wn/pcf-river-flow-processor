using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters;
using App.Metrics.Timer;

namespace RiverFlowProcessor
{
    public class MetricsFormatter : IMetricsOutputFormatter
    {
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue(
            "text", "vnd.custom.metrics", "v1", "plain");

        public MetricFields MetricFields { get; set; }

        public async Task WriteAsync(
            Stream output,
            MetricsDataValueSource snapshot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var writer = new StreamWriter(output))
            {
                foreach (var timer in snapshot.Contexts.SelectMany(c => c.Timers))
                {
                    await writer.WriteLineAsync(
                        $"{timer.Name} - " +
                        $"{timer.Value.Rate.Count} total. Rates: " +
                        $"{timer.Value.Rate.MeanRate:0} mean, " +
                        $"{timer.Value.Rate.OneMinuteRate:0} 1-min, " +
                        $"{timer.Value.Rate.FiveMinuteRate:0} 5-min. #metric");
                }

                foreach (var counter in snapshot.Contexts.SelectMany(c => c.Counters))
                {
                    await writer.WriteLineAsync(
                        $"{counter.Name} - " +
                        $"{counter.Value.Count} total.");
                }
            }
        }
    }
}