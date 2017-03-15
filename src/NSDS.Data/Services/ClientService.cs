using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class ClientService : IClientsService
	{
		private readonly ApplicationDbContext context;

		public ClientService(ApplicationDbContext context)
		{
			this.context = context;
		}

		public Client AddClient(Client cli)
		{
			var dbClient = new ClientDataModel
			{
				Name = cli.Name,
				Address = cli.Address,
				Enabled = cli.Enabled,
			};
			this.context.Clients.Add(dbClient);
			this.context.SaveChanges();
			return dbClient;
		}

		public void RemoveClient(Client client)
		{
			var cli = this.context.Clients.FirstOrDefault(x => x.Name == client.Name && x.Address == client.Address);
			if (cli == null)
			{
				return;
			}
			if (cli.Pool != null)
			{
				var pool = this.context.Pools.First(p => p.Id == cli.Pool.Id);
				pool.Clients.Remove(cli);
			}
			this.context.Clients.Remove(cli);
			this.context.SaveChanges();
		}

		public void Dispose()
		{
			this.context.Dispose();
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.context.Clients;
		}

		public void AddClientToPool(Client cli, int poolId)
		{
			var pool = this.context.Pools.First(p => p.Id == poolId);
			pool.Clients.Add(this.context.Clients.Single(x => x.Name == cli.Name && x.Address == cli.Address));
			this.context.SaveChanges();
		}

		public IEnumerable<Client> GetClientsInPool(int poolId)
		{
			return this.context.Clients.Where(c => c.Pool.Id == poolId)
				.Include(c => c.Modules).ThenInclude(m => m.Module)
				.ToArray();
		}
	}
}
