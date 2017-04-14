using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class DeploymentStorage : IDeploymentStorage
	{
		private readonly IServiceProvider services;

		public DeploymentStorage(IServiceProvider services)
		{
			this.services = services;
		}

		public void Dispose()
		{
			//this.context.Dispose();
		}

		public Deployment GetDeployment(string name)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return context.Deployments.Include("DeploymentCommands.Command").Single(x => x.Name == name).ToDeployment(new MappingContext());
			}
		}

		private IQueryable<DeploymentDataModel> GetSelectDeployments(ApplicationDbContext context)
		{
			return from dc in context.DeploymentCommands
				   join c in context.Commands on dc.CommandName equals c.Name
				   join d in context.Deployments on dc.DeploymentId equals d.Id
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
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var mappingContext = new MappingContext();
				return this.GetSelectDeployments(context).Select(x => x.ToDeployment(mappingContext)).ToArray();
			}
		}
	}
}
