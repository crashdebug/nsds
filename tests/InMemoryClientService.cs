using System.Collections.Generic;
using System.Linq;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	class InMemoryClientService : IClientsService
	{
		private List<Client> clients = new List<Client>();

		public InMemoryClientService(params Client[] clients)
		{
			// KLUDGE: Set Module.Client to Client.Module manually.
			foreach (var cli in clients)
			{
				foreach (var mod in cli.Modules)
				{
					mod.Client = cli;
				}
			}
			this.clients.AddRange(clients);
		}

		public void AddClient(Client client)
		{
			this.clients.Add(client);
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.clients.AsReadOnly();
		}

		public IEnumerable<Client> GetClientsInPool(int poolId)
		{
			return this.clients.Where(x => x.PoolId == poolId);
		}
	}
}
