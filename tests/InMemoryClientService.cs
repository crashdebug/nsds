using System;
using System.Collections.Generic;
using System.Linq;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	class InMemoryClientService : IModuleStorage, IClientsStorage, IPoolStorage
	{
		private List<ClientModel> clients = new List<ClientModel>();
		private List<PoolModel> pools = new List<PoolModel>();
		private List<ModuleModel> modules = new List<ModuleModel>();

		public InMemoryClientService(params ClientModel[] clients)
		{
			this.clients.AddRange(clients);
		}

		public void AddClient(Client client)
		{
			this.clients.Add(new ClientModel
			{
				Name = client.Name,
				Address = client.Address,
				Enabled = client.Enabled,
				Modules = new List<ClientModule>()
			});
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.clients.AsReadOnly();
		}

		public IEnumerable<Client> GetClientsInPool(int poolId)
		{
			return this.pools.First(x => x.Id == poolId).Clients;
		}

		public IEnumerable<Pool> GetPools()
		{
			return this.pools.AsEnumerable<Pool>();
		}

		public void AddPool(Pool pool)
		{
			this.pools.Add(new PoolModel
			{
				Id = this.pools.Count + 1,
				Name = pool.Name
			});
		}

		public IEnumerable<ClientModule> GetClientModules(Client client)
		{
			var model = client as ClientModel;
			if (model == null)
			{
				model = this.clients.Single(x => x.Name == client.Name);
			}
			return model.Modules.AsEnumerable();
		}

		public void Dispose()
		{
			this.clients.Clear();
			this.pools.Clear();
		}

		public Module GetModule(string name)
		{
			return this.modules.Single(x => x.Name == name);
		}

		public Client GetClient(string name)
		{
			return this.clients.Single(x => x.Name == name);
		}

		public bool UpdateModuleVersion(Client client, Module module, BaseVersion version)
		{
			client.Modules.Single(x => x.Module == module).Version = version;
			return true;
		}

		public ClientModule GetClientModule(string clientName, string moduleName)
		{
			return this.clients.Where(x => x.Name == clientName).Single().Modules.Where(x => x.Module.Name == moduleName).Single();
		}
	}
}
