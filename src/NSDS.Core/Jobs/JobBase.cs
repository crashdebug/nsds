using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NSDS.Core.Jobs
{
	public enum JobStatus
	{
		Unknown = 0,
		Success = 1,
		Error = 2,
		Running = 3,
	}

	public abstract class JobBase
	{
        protected readonly ILogger Log;

        public DateTime LastRun { get; private set; }

		private Stack<TimeSpan> intervals = new Stack<TimeSpan>();

		public TimeSpan Interval
		{
			get { return this.intervals.Peek(); }
			set
			{
				this.intervals.Clear();
				this.intervals.Push(value);
			}
		}

		public JobStatus Status { get; private set; }

		protected JobBase(ILogger log = null)
		{
			this.Log = log;
			this.LastRun = DateTime.MinValue;
			this.Interval = TimeSpan.FromMinutes(10);
		}

		public void Run()
		{
			var now = DateTime.Now;
			if (now - this.Interval > this.LastRun)
			{
				try
				{
					this.Status = JobStatus.Running;
					this.RunOnce();
					this.LastRun = now;
					this.Status = JobStatus.Success;
					while (this.intervals.Count > 1)
					{
						this.intervals.Pop();
					}
				}
				catch (Exception ex)
				{
					this.Status = JobStatus.Error;
					if (this.Log != null)
					{
						this.Log.LogError("An exception occurred while running '{0}':\n{1}", this.GetType().Name, ex.ToString());
					}
					this.intervals.Push(new TimeSpan(this.Interval.Ticks * 2));
				}
			}
		}

		protected abstract void RunOnce();
	}
}
