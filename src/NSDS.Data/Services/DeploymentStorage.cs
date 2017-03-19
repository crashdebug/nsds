using System.Collections.Generic;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Data.Services
{
	public class DeploymentStorage : IDeploymentStorage
	{
		private readonly ApplicationDbContext context;

		public DeploymentStorage(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void Dispose()
		{
			this.context.Dispose();
		}

		public IEnumerable<Deployment> GetDeployments()
		{
			return this.context.Deployments;
		}
	}
}
