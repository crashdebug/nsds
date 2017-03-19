using System.Collections.Generic;
using System.Linq;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	class InMemoryClientService : IModuleStorage, IClientsService, IPoolStorage
	{
		private List<ClientModel> clients = new List<ClientModel>();
		private List<PoolModel> pools = new List<PoolModel>();

		public InMemoryClientService(params ClientModel[] clients)
		{
			this.clients.AddRange(clients);
		}

		public Client AddClient(Client client)
		{
			var model = new ClientModel
			{
				Name = client.Name,
				Address = client.Address,
				Enabled = client.Enabled,
				Modules = new List<Module>()
			};
			this.clients.Add(model);
			return model;
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

		public Pool AddPool(Pool pool)
		{
			var dbPool = new PoolModel
			{
				Id = this.pools.Count + 1,
				Name = pool.Name
			};
			this.pools.Add(dbPool);
			return dbPool;
		}

		public IEnumerable<Module> GetClientModules(Client client)
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
	}
}
