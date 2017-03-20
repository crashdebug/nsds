using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

		public Deployment GetDeployment(string name)
		{
			return this.context.Deployments.Include(x => x.DeploymentCommands).Single(x => x.Name == name);
		}

		public IEnumerable<Deployment> GetDeployments()
		{
			return this.context.Deployments;
		}
	}
}
