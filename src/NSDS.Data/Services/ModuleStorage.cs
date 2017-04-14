using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Data.Services
{
	public class ModuleStorage : IModuleStorage
    {
		private readonly IServiceProvider services;

		public ModuleStorage(IServiceProvider services)
		{
			this.services = services;
		}

		public void Dispose()
		{
			//this.context.Dispose();
		}

		public IEnumerable<ClientModule> GetClientModules(int clientId)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var dbClient = context.Clients.Include("ClientModules.Module").Single(x => x.Id == clientId);
				return dbClient.ClientModules.Select(x => new ClientModule { Module = x.Module.ToModule(new MappingContext()), Version = x.Version }).ToArray();
			}
		}

		public Module GetModule(string name)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return context.Modules.SingleOrDefault(x => x.Name == name).ToModule(new MappingContext());
			}
		}
	}
}
