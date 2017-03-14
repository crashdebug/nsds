using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data
{
	public class ApplicationDbContext : DbContext, IPoolService, IClientsService, IModuleService
	{
		public ApplicationDbContext(DbContextOptions context) :
			base(context)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			builder.Entity<BaseVersion>().ToTable("Versions").HasKey(v => v.Version);
			builder.Entity<DateVersion>();
			builder.Entity<ModuleDataModel>().Ignore(x => x.Version);
			builder.Entity<ClientModuleDataModel>().HasKey(x => new { x.ClientId, x.ModuleId });
		}

		public void Seed()
		{
			this.Database.Migrate();

			var uiVersionOld = this.Versions.FirstOrDefault(x => x.Version == "2017-02-14 00:00:00");
			if (uiVersionOld == null)
			{
				uiVersionOld = new DateVersion("2017-02-14 00:00:00");
				this.Versions.Add(uiVersionOld);
				this.SaveChanges();
			}

			var uiVersionNew = this.Versions.FirstOrDefault(x => x.Version == "2017-03-14 00:00:00");
			if (uiVersionNew == null)
			{
				uiVersionNew = new DateVersion("2017-03-14 00:00:00");
				this.Versions.Add(uiVersionNew);
				this.SaveChanges();
			}

			//var cartVersion = this.Versions.FirstOrDefault(x => x.Version == "2017-03-14 12:34:56");
			//if (cartVersion == null)
			//{
			//	cartVersion = new DateVersion("2017-03-14 12:34:56");
			//	this.Versions.Add(cartVersion);
			//	this.SaveChanges();
			//}

			var prodPool = this.Pools.FirstOrDefault(x => x.Name == "PROD");
			if (prodPool == null)
			{
				prodPool = new PoolDataModel { Name = "PROD" };
				this.Pools.Add(prodPool);
			}
			var qaPool = this.Pools.FirstOrDefault(x => x.Name == "QA");
			if (qaPool == null)
			{
				qaPool = new PoolDataModel { Name = "QA" };
				this.Pools.Add(qaPool);
			}
			this.SaveChanges();

			var uiModule = this.Modules.FirstOrDefault(x => x.Name == "UI");
			if (uiModule == null)
			{
				uiModule = new ModuleDataModel
				{
					Name = "UI",
					Endpoint = "/ui/version"
				};
				this.Modules.Add(uiModule);
				this.SaveChanges();
			}

			if (!this.Clients.Any(x => x.Name == "16"))
			{
				var client = new ClientDataModel
				{
					Name = "16",
					Address = "127.0.0.1:8000",
					PoolId = this.Pools.Single(x => x.Name == "QA").Id
				};
				this.Clients.Add(client);
				this.ClientModules.Add(new ClientModuleDataModel
				{
					Client = client,
					Module = uiModule,
					Version = uiVersionOld
				});
			}
			if (!this.Clients.Any(x => x.Name == "24"))
			{
				var client = new ClientDataModel
				{
					Name = "24",
					Address = "127.0.0.1:8001",
					PoolId = this.Pools.Single(x => x.Name == "QA").Id
				};
				this.Clients.Add(client);
				this.ClientModules.Add(new ClientModuleDataModel
				{
					Client = client,
					Module = uiModule,
					Version = uiVersionNew
				});
			}
			this.SaveChanges();
		}

        public DbSet<PoolDataModel> Pools { get; set; }
		public DbSet<ClientDataModel> Clients { get; set; }
		public DbSet<ModuleDataModel> Modules { get; set; }
		public DbSet<ClientModuleDataModel> ClientModules { get; set; }
		public DbSet<PackageDataModel> Packages { get; set; }
		public DbSet<BaseVersion> Versions { get; set; }

		public IEnumerable<Pool> GetPools()
        {
            return this.Pools.ToArray();
        }

		public Pool AddPool(Pool pool)
		{
			var dbPool = new PoolDataModel
			{
				Name = pool.Name
			};
			this.Pools.Add(dbPool);
			this.SaveChanges();
			return dbPool;
		}

		public void RemovePool(int poolId)
		{
			var pool = this.Pools.First(p => p.Id == poolId);
			this.Pools.Remove(pool);
			this.SaveChanges();
		}

		public IEnumerable<Client> GetClientsInPool(int poolId)
		{
			return this.Clients.Where(c => c.Pool.Id == poolId)
				.Include(c => c.Modules).ThenInclude(m => m.Module)
				.ToArray();
		}

		public void AddClientToPool(Client cli, int poolId)
		{
			var pool = this.Pools.First(p => p.Id == poolId);
			pool.Clients.Add(this.Clients.First(x => x.Name == cli.Name && x.Address == cli.Address));
			this.SaveChanges();
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.Clients;
		}

		public Client AddClient(Client cli)
		{
			var dbClient = new ClientDataModel
			{
				Name = cli.Name,
				Address = cli.Address,
				Enabled = cli.Enabled,
			};
			this.Clients.Add(dbClient);
			this.SaveChanges();
			return dbClient;
		}

		public void RemoveClient(Client client)
		{
			var cli = this.Clients.FirstOrDefault(x => x.Name == client.Name && x.Address == client.Address);
			if (cli == null)
			{
				return;
			}
			if (cli.Pool != null)
			{
				var pool = this.Pools.First(p => p.Id == cli.Pool.Id);
				pool.Clients.Remove(cli);
			}
			this.Clients.Remove(cli);
			this.SaveChanges();
		}

		public IEnumerable<Module> GetClientModules(Client client)
		{
			var dbClient = client as ClientDataModel;
			if (dbClient == null)
			{
				dbClient = this.Clients.Single(x => x.Name == client.Name && x.Address == client.Address);
			}
			return dbClient.Modules.Select(x => x.Module).AsEnumerable<Module>().ToArray();
		}
	}
}
