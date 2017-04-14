using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class ClientStorage : IClientsStorage
	{
		private readonly ILogger logger;
		private readonly IServiceProvider services;

		public ClientStorage(IServiceProvider services, ILogger logger = null)
		{
			this.services = services;
			this.logger = logger;
		}

		public void AddClient(Client cli)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				context.Clients.Add(new ClientDataModel
				{
					Name = cli.Name,
					Address = cli.Address,
					Enabled = cli.Enabled,
					Created = DateTime.UtcNow,
				});
				context.SaveChanges();
			}
		}

		public void RemoveClient(Client client)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var cli = context.Clients.FirstOrDefault(x => x.Name == client.Name && x.Address == client.Address);
				if (cli == null)
				{
					return;
				}
				if (cli.Pool != null)
				{
					var pool = context.Pools.First(p => p.Id == cli.Pool.Id);
					pool.Clients.Remove(cli);
				}
				context.Clients.Remove(cli);
				context.SaveChanges();
			}
		}

		private IQueryable<ClientDataModel> SelectClients(ApplicationDbContext context)
		{
			return context.Clients
				.Include("ClientModules.Version")
				.Include("ClientModules.Module.Package.Version")
				.Include("ClientModules.Module.Deployment.DeploymentCommands.Command")
			;
		}

		public Client GetClient(string name)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return this.SelectClients(context)
					.Single(x => x.Name == name)
					.ToClient(new MappingContext());
			}
		}

		public IEnumerable<Client> GetAllClients()
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var mappingContext = new MappingContext();
				return this.SelectClients(context)
					.Select(x => x.ToClient(mappingContext))
					.ToList();
			}
		}

		public void AddClientToPool(Client cli, int poolId)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var pool = context.Pools.First(p => p.Id == poolId);
				pool.Clients.Add(context.Clients.Single(x => x.Name == cli.Name && x.Address == cli.Address));
				context.SaveChanges();
			}
		}

		public IEnumerable<Client> GetClientsInPool(int poolId)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var mappingContext = new MappingContext();
				return context.Clients.Where(c => c.Pool.Id == poolId)
					.Include(c => c.ClientModules).ThenInclude(m => m.Module)
					.Select(c => c.ToClient(mappingContext))
					.ToArray();
			}
		}

		public bool UpdateModuleVersion(Client client, Module module, BaseVersion version)
		{
			try
			{
				using (var context = this.services.GetService<ApplicationDbContext>())
				{
					var c = context.ClientModules.FirstOrDefault(x => x.Client.Name == client.Name && x.Module.Name == module.Name);
					if (c == null)
					{
						c = new ClientModuleDataModel
						{
							Client = context.Clients.Single(x => x.Name == client.Name),
							Module = context.Modules.Single(x => x.Name == module.Name),
						};
						context.ClientModules.Add(c);
					}
					var v = context.Versions.FirstOrDefault(x => x.Version == version.Version);
					if (v == null)
					{
						v = context.Versions.Add(version).Entity;
					}
					c.Version = v;
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception ex)
			{
				this.logger?.LogError("Could not update module '{0}' version for client '{1}' to '{2}':\n{3}", module.Name, client.Name, version.ToString(), ex.ToString());
				return false;
			}
		}

		public ClientModule GetClientModule(string clientName, string moduleName)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var model = context.ClientModules
					.Include("Client")
					.Include("Module.Deployment.DeploymentCommands.Command")
					.Include("Module.Package.Version")
					.Include("Version")
					.Where(x => x.Client.Name == clientName && x.Module.Name == moduleName)
					.SingleOrDefault()
				;
				return new ClientModule
				{
					Module = model.Module.ToModule(new MappingContext()),
					Version = model.Version,
				};
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
//					this.context.Dispose();
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
