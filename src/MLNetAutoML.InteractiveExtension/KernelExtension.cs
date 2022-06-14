using Microsoft.AspNetCore.Html;
using Microsoft.Data.Analysis;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using XPlot.Plotly;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;


namespace MLNetAutoML.InteractiveExtension
{
    public class KernelExtension : IKernelExtension
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public Task OnLoadAsync(Kernel kernel)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Formatter.Register<NotebookMonitor>((monitor, writer) =>
            {
                WriteSummary(monitor, writer);
                WriteChart(monitor, writer);
                WriteTable(monitor, writer);
            }, "text/html");

            return Task.CompletedTask;
        }

        private static void WriteSummary(NotebookMonitor monitor, TextWriter writer)
        {

            var summary = new List<IHtmlContent>();

            if (monitor.BestTrial != null)
            {
                summary.Add(h3("Best Run"));
                summary.Add(p($"Trial: {monitor.BestTrial.TrialSettings.TrialId}"));
                summary.Add(p($"Trainer: {monitor.BestTrial.TrialSettings.Pipeline}".Replace("Unknown=>", "")));
            }
            if (monitor.ActiveTrial != null)
            {

                var activeRunParam = JsonSerializer.Serialize(monitor.ActiveTrial.Parameter, new JsonSerializerOptions() { WriteIndented = true, });

                summary.Add(h3("Active Run"));
                summary.Add(p($"Trial: {monitor.ActiveTrial.TrialId}"));
                summary.Add(p($"Trainer: {monitor.ActiveTrial.Pipeline}".Replace("Unknown=>", "")));
                summary.Add(p($"Parameters: {activeRunParam}"));
            }

            writer.Write(div(summary));
        }

        private static void WriteChart(NotebookMonitor monitor, TextWriter writer)
        {
            var chart = Chart.Plot(
                                        new Scatter()
                                        {
                                            x = monitor.CompletedTrials.Select(x => x.TrialSettings.TrialId),
                                            y = monitor.CompletedTrials.Select(x => x.Metric),
                                            mode = "markers",
                                        }
                                    );

            var layout = new Layout.Layout() { title = $"Plot metrics over trials." };
            chart.WithLayout(layout);
            chart.Width = 500;
            chart.Height = 500;
            chart.WithXTitle("Trial");
            chart.WithYTitle("Metric");
            chart.WithLegend(false);


            Formatter.GetPreferredFormatterFor(typeof(PlotlyChart), "text/html").Format(chart, writer);
        }

        private static void WriteTable(NotebookMonitor notebookMonitor, TextWriter writer)
        {
            Formatter.GetPreferredFormatterFor(typeof(DataFrame), "text/html").Format(notebookMonitor.DataFrame, writer);
        }
    }
}