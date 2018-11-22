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
                        $"{timer.Value.Rate.Count} total, " +
                        $"{timer.Value.Rate.OneMinuteRate:0.000} / min, " +
                        $"{timer.Value.Rate.MeanRate:0.000} mean.  #metric");
                }
                
            }

            return Task.CompletedTask;
        }
    }
}