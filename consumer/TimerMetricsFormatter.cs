using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters;
using App.Metrics.Timer;

namespace RiverFlowProcessor
{
    public class TimerMetricsFormatter : IMetricsOutputFormatter
    {
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue(
            "text", "vnd.custom.metrics", "v1", "plain");

        public MetricFields MetricFields { get; set; }

        public Task WriteAsync(Stream output,
            MetricsDataValueSource snapshot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var writer = new StreamWriter(output))
            {
                foreach (var timer in snapshot.Contexts.SelectMany(c => c.Timers))
                {
                    writer.WriteLineAsync(
                        $"{timer.Name} - " +
                        $"{timer.Value.Rate.Count} total. Rates: " +
                        $"{timer.Value.Rate.OneMinuteRate:0.0} 1-min, " +
                        $"{timer.Value.Rate.FiveMinuteRate:0.0} 5-min. #metric");
                }
            }

            return Task.CompletedTask;
        }
    }
}