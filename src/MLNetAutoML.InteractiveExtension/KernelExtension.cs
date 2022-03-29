using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XPlot.Plotly;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;


namespace MLNetAutoML.InteractiveExtension
{
    public class KernelExtension : IKernelExtension
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task OnLoadAsync(Kernel kernel)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Formatter.Register<NotebookMonitor>((monitor, writer) =>
            {
                WriteSummaryAndChart(monitor, writer);
                WriteRunTable(monitor, writer);
            }, "text/html");
        }

        private static void WriteSummaryAndChart(NotebookMonitor monitor, TextWriter writer)
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


            var scriptJs = chart.GetInlineJS().Replace("<script>", String.Empty).Replace("</script>", String.Empty);


            var bestRun = monitor.BestTrial == null ? "" :
            $@"
	<h3>Best Run</h3>
	<p>
		Trial: {monitor.BestTrial.TrialSettings.TrialId} <br>
        Pipeline: {monitor.BestTrial.TrialSettings.Pipeline}<br>
	</p>
	";

            // var activeRunParam = monitor.ActiveTrial == null ? "" : JsonSerializer.Serialize(monitor.ActiveTrial.Parameter, new JsonSerializerOptions() { WriteIndented = true, });
            var activeRun = monitor.ActiveTrial == null ? "" :
            $@"
	<h3>Active Run</h3>
	<p>
		Trial: {monitor.ActiveTrial.TrialId} <br>
		Pipeline: {monitor.ActiveTrial.Pipeline}<br>
	</p>
	";


            writer.Write($@"
<div>
	{bestRun}
	{activeRun}
</div>
<div style=""width: {chart.Width}px; height: {chart.Height}px;"" id=""{chart.Id}"">
</div>
<script type=""text/javascript"">
var renderPlotly = function() {{
    var xplotRequire = require.config({{context:'xplot-3.0.1',paths:{{plotly:'https://cdn.plot.ly/plotly-1.49.2.min'}}}}) || require;
    xplotRequire(['plotly'], function(Plotly) {{ 

{scriptJs}
        
}});
}};
// ensure `require` is available globally
if ((typeof(require) !==  typeof(Function)) || (typeof(require.config) !== typeof(Function))) {{
    let require_script = document.createElement('script');
    require_script.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js');
    require_script.setAttribute('type', 'text/javascript');
    require_script.onload = function() {{
        renderPlotly();
    }};

    document.getElementsByTagName('head')[0].appendChild(require_script);
}}
else {{
    renderPlotly();
}}
</script>
");
        }

        private static void WriteRunTable(NotebookMonitor notebookMonitor, TextWriter writer)
        {
                const int maxRowCount = 10000;
                const int rowsPerPage = 25;

                var uniqueId = DateTime.Now.Ticks;

                var header = new List<IHtmlContent>
                {
                    th(i("Trial")),
                    th(i("Metric")),
                    th(i("Pipeline"))
                };


                if (notebookMonitor.CompletedTrials.Count > rowsPerPage)
                {
                    var maxMessage = notebookMonitor.CompletedTrials.Count > maxRowCount ? $" (showing a max of {maxRowCount} rows)" : string.Empty;
                    var title = h3[style: "text-align: center;"]($"Trials - {notebookMonitor.CompletedTrials.Count} rows {maxMessage}");

                    // table body
                    var rowCount = Math.Min(maxRowCount, notebookMonitor.CompletedTrials.Count);
                    var rows = new List<List<IHtmlContent>>();
                    for (var index = 0; index < rowCount; index++)
                    {
   
                           var cells = new List<IHtmlContent>
                        {
                            td(notebookMonitor.CompletedTrials[index].TrialSettings.TrialId.ToString()),
                            td(notebookMonitor.CompletedTrials[index].Metric.ToString()),
                            td(notebookMonitor.CompletedTrials[index].TrialSettings.Pipeline.ToString()),
                        };
                        rows.Add(cells);
                    }

                    //navigator
                    var footer = new List<IHtmlContent>();
                    BuildHideRowsScript(uniqueId);

                    var paginateScriptFirst = BuildHideRowsScript(uniqueId) + GotoPageIndex(uniqueId, 0) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptFirst]("⏮"));

                    var paginateScriptPrevTen = BuildHideRowsScript(uniqueId) + UpdatePageIndex(uniqueId, -10, (rowCount - 1) / rowsPerPage) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptPrevTen]("⏪"));

                    var paginateScriptPrev = BuildHideRowsScript(uniqueId) + UpdatePageIndex(uniqueId, -1, (rowCount - 1) / rowsPerPage) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptPrev]("◀️"));

                    footer.Add(b[style: "margin: 2px;"]("Page"));
                    footer.Add(b[id: $"page_{uniqueId}", style: "margin: 2px;"]("1"));

                    var paginateScriptNext = BuildHideRowsScript(uniqueId) + UpdatePageIndex(uniqueId, 1, (rowCount - 1) / rowsPerPage) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptNext]("▶️"));

                    var paginateScriptNextTen = BuildHideRowsScript(uniqueId) + UpdatePageIndex(uniqueId, 10, (rowCount - 1) / rowsPerPage) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptNextTen]("⏩"));

                    var paginateScriptLast = BuildHideRowsScript(uniqueId) + GotoPageIndex(uniqueId, (rowCount - 1) / rowsPerPage) + BuildPageScript(uniqueId, rowsPerPage);
                    footer.Add(button[style: "margin: 2px;", onclick: paginateScriptLast]("⏭️"));

                    //table
                    var t = table[id: $"table_{uniqueId}"](
                        caption(title),
                        thead(tr(header)),
                        tbody(rows.Select(r => tr[style: "display: none"](r))),
                        tfoot(tr(td[colspan: notebookMonitor.CompletedTrials.Count + 1, style: "text-align: center;"](footer)))
                    );
                    writer.Write(t);

                    //show first page
                    writer.Write($"<script>{BuildPageScript(uniqueId, rowsPerPage)}</script>");
                }
                else
                {
                    var rows = new List<List<IHtmlContent>>();
                    for (var index = 0; index < notebookMonitor.CompletedTrials.Count; index++)
                    {
                        var cells = new List<IHtmlContent>
                        {
                            td(notebookMonitor.CompletedTrials[index].TrialSettings.TrialId.ToString()),
                            td(notebookMonitor.CompletedTrials[index].Metric.ToString()),
                            td(notebookMonitor.CompletedTrials[index].TrialSettings.Pipeline.ToString()),
                        };
                        rows.Add(cells);
                    }

                    //table
                    var t = table[id: $"table_{uniqueId}"](
                        thead(tr(header)),
                        tbody(rows.Select(r => tr(r)))
                    );
                    writer.Write(t);
                }
        }

        private static string BuildHideRowsScript(long uniqueId)
        {
            var script = $"var allRows = document.querySelectorAll('#table_{uniqueId} tbody tr:nth-child(n)'); ";
            script += "for (let i = 0; i < allRows.length; i++) { allRows[i].style.display='none'; } ";
            return script;
        }

        private static string BuildPageScript(long uniqueId, int size)
        {
            var script = $"var page = parseInt(document.querySelector('#page_{uniqueId}').innerHTML) - 1; ";
            script += $"var pageRows = document.querySelectorAll(`#table_{uniqueId} tbody tr:nth-child(n + ${{page * {size} + 1 }})`); ";
            script += $"for (let j = 0; j < {size}; j++) {{ pageRows[j].style.display='table-row'; }} ";
            return script;
        }

        private static string GotoPageIndex(long uniqueId, long page)
        {
            var script = $"document.querySelector('#page_{uniqueId}').innerHTML = {page + 1}; ";
            return script;
        }

        private static string UpdatePageIndex(long uniqueId, int step, long maxPage)
        {
            var script = $"var page = parseInt(document.querySelector('#page_{uniqueId}').innerHTML) - 1; ";
            script += $"page = parseInt(page) + parseInt({step}); ";
            script += $"page = page < 0 ? 0 : page; ";
            script += $"page = page > {maxPage} ? {maxPage} : page; ";
            script += $"document.querySelector('#page_{uniqueId}').innerHTML = page + 1; ";
            return script;
        }

    }
}