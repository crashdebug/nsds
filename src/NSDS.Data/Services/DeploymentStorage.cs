using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

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
			return this.context.Deployments.Include("DeploymentCommands.Command").Single(x => x.Name == name).ToDeployment();
			//var @select = GetSelectDeployments();
			//return @select.Single(x => x.Name == name).ToDeployment();
		}

		private IQueryable<DeploymentDataModel> GetSelectDeployments()
		{
			return from dc in this.context.DeploymentCommands
				   join c in this.context.Commands on dc.CommandName equals c.Name
				   join d in this.context.Deployments on dc.DeploymentId equals d.Id
				   group new { dc, c } by d into g
				   select new DeploymentDataModel
				   {
					   Name = g.Key.Name,
					   Created = g.Key.Created,
					   DeploymentCommands = g.Select(x => new DeploymentCommandsDataModel(x.dc, x.c)).ToList()
				   };
		}

		public IEnumerable<Deployment> GetDeployments()
		{
			return this.GetSelectDeployments().Select(x => x.ToDeployment());
		}
	}
}
