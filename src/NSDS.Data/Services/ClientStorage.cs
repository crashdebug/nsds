using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class ClientStorage : IClientsStorage
	{
		private readonly ApplicationDbContext context;

		public ClientStorage(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void AddClient(Client cli)
		{
			this.context.Clients.Add(new ClientDataModel
			{
				Name = cli.Name,
				Address = cli.Address,
				Enabled = cli.Enabled,
				Created = DateTime.UtcNow,
			});
			this.context.SaveChanges();
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

		public Client GetClient(int id)
		{
			return this.context.Clients.Include(x => x.ClientModules).ThenInclude(m => m.Module).Single(x => x.Id == id).ToClient();
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.context.Clients.Include("ClientModules.Module").Select(x => x.ToClient());
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
				.Include(c => c.ClientModules).ThenInclude(m => m.Module)
				.Select(c => c.ToClient())
				.ToArray();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.context.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ClientStorage() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			this.Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion
	}
}
