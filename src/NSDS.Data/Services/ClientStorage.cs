using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class ClientStorage : IClientsStorage
	{
		private readonly ApplicationDbContext context;

		public ClientStorage(ApplicationDbContext context, ILogger logger = null)
		{
			this.context = context;
			this.logger = logger;
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
			return this.context.Clients
				.Include("ClientModules.Version")
				.Include("ClientModules.Module.Deployment.DeploymentCommands.Command")
				.Select(x => x.ToClient());
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
		private readonly ILogger logger;

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

		public bool UpdateModuleVersion(Client client, Module module, BaseVersion version)
		{
			try
			{
				var c = this.context.ClientModules.FirstOrDefault(x => x.Client.Name == client.Name && x.Module.Name == module.Name);
				if (c == null)
				{
					c = new ClientModuleDataModel
					{
						Client = this.context.Clients.Single(x => x.Name == client.Name),
						Module = this.context.Modules.Single(x => x.Name == module.Name),
					};
					this.context.ClientModules.Add(c);
					//this.context.SaveChanges();
				}
				var v = this.context.Versions.FirstOrDefault(x => x.Version == version.Version);
				if (v == null)
				{
					v = this.context.Versions.Add(version).Entity;
					//this.context.SaveChanges();
				}
				c.Version = v;
				this.context.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				this.logger?.LogError("Could not update module '{0}' version for client '{1}' to '{2}':\n{3}", module.Name, client.Name, version.ToString(), ex.ToString());
				return false;
			}
		}

		#endregion
	}
}
