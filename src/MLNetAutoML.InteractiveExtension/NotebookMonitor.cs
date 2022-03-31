using Microsoft.DotNet.Interactive;
using Microsoft.ML.AutoML;
using System.Collections.Generic;
using Microsoft.Data.Analysis;
using Apache.Arrow;
using System;
using System.Threading.Tasks;

namespace MLNetAutoML.InteractiveExtension
{
    public class NotebookMonitor : IMonitor
	{
		private DisplayedValue? ValueToUpdate;
		private DateTime _lastUpdate = DateTime.MinValue;

		public TrialResult? BestTrial { get; set; }
		public TrialResult? MostRecentTrial { get; set; }
		public TrialSettings? ActiveTrial { get; set; }
		public List<TrialResult> CompletedTrials { get; set; }
		public DataFrame DataFrame { get; set; }

		public NotebookMonitor()
		{
			this.CompletedTrials = new List<TrialResult>();
			this.DataFrame = new DataFrame(new PrimitiveDataFrameColumn<int>("Trial"), new PrimitiveDataFrameColumn<float>("Metric"), new StringDataFrameColumn("Pipeline"));
		}

		public void ReportBestTrial(TrialResult result)
		{
			this.BestTrial = result;
			Update();
		}

		public void ReportCompletedTrial(TrialResult result)
		{
			this.MostRecentTrial = result;
			this.CompletedTrials.Add(result);

			this.DataFrame.Append(new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("Trial",result.TrialSettings.TrialId),
				new KeyValuePair<string, object>("Metric", result.Metric),
				new KeyValuePair<string, object>("Pipeline",result.TrialSettings.Pipeline.ToString()),
			}, true);
			Update();
		}

		public void ReportFailTrial(TrialResult result)
		{
			// TODO figure out what to do with failed trials.
			Update();
		}

		public void ReportRunningTrial(TrialSettings setting)
		{
			this.ActiveTrial = setting;
			Update();
		}

		public void Update()
		{
			Task.Run(async () =>
			{
				int timeRemaining = 2000 - (int)(DateTime.Now - this._lastUpdate).TotalMilliseconds;

				if (timeRemaining > 0)
				{
					await Task.Delay(timeRemaining);
					Update();
				}
				else
				{
					if (this.ValueToUpdate != null)
					{
						this._lastUpdate = DateTime.Now;
						this.ValueToUpdate.Update(this);
					}
				}
			});

		}

		public void SetUpdate(DisplayedValue valueToUpdate)
		{
			this.ValueToUpdate = valueToUpdate;
		}
	}
}
