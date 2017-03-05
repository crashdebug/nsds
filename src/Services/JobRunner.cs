using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Jobs;

namespace WebApplication.Services
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
			Task.Run(new Action(this.Run), this.token.Token);
		}

		public void Stop()
		{
			this.token.Cancel();
		}

		private void Run()
		{
			while (!this.token.IsCancellationRequested)
			{
				foreach (var job in this.jobs)
				{
					job.Run();
				}
				Task.Delay(100);
			}
		}
	}
}