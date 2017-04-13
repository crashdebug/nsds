using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSDS.Core.Jobs;

namespace NSDS.Core.Services
{
	public class JobRunner
	{
        private readonly IEnumerable<JobBase> jobs;
        private CancellationTokenSource token;

        public JobRunner(IEnumerable<JobBase> jobs)
		{
			this.jobs = jobs;
		}

		public void Start()
		{
			this.token = new CancellationTokenSource();
			Task.Run(new Action(this.RunAsync), this.token.Token);
		}

		public void Stop()
		{
			this.token.Cancel();
		}

		private async void RunAsync()
		{
			while (!this.token.IsCancellationRequested)
			{
				foreach (var job in this.jobs)
				{
					job.Run();
				}
				await Task.Delay(100);
			}
		}
	}
}