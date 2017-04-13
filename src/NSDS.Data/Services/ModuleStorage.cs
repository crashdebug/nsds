using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

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

		public IEnumerable<Module> GetClientModules(int clientId)
		{
			var dbClient = this.context.Clients.Include("ClientModules.Module").Single(x => x.Id == clientId);
			return dbClient.ClientModules.Select(x => x.Module.ToModule()).AsEnumerable().ToArray();
		}

		public Module GetModule(string name)
		{
			return this.context.Modules.SingleOrDefault(x => x.Name == name)?.ToModule();
		}
	}
}
