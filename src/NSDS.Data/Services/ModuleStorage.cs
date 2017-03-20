using System;
using System.Collections.Generic;
using System.Linq;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class ModuleStorage : IModuleStorage
    {
		private readonly ApplicationDbContext context;

		public ModuleStorage(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void Dispose()
		{
			this.context.Dispose();
		}

		public IEnumerable<Module> GetClientModules(Client client)
		{
			var dbClient = client as ClientDataModel;
			if (dbClient == null)
			{
				dbClient = this.context.Clients.Single(x => x.Name == client.Name && x.Address == client.Address);
			}
			return dbClient.ClientModules.Select(x => x.Module).AsEnumerable<Module>().ToArray();
		}

		public Module GetModule(int moduleId)
		{
			return this.context.Modules.Single(x => x.Id == moduleId);
		}
	}
}
