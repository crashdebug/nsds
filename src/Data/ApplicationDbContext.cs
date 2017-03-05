using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Data
{
	public class ApplicationDbContext : DbContext, IPoolService, IClientsService
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
		}

		public void Seed()
		{
			if (!this.Pools.Any(x => x.Name == "PROD"))
				this.Pools.Add(new Pool { Name = "PROD" });
			if (!this.Pools.Any(x => x.Name == "QA"))
				this.Pools.Add(new Pool { Name = "QA" });
			this.SaveChanges();

			if (!this.Clients.Any(x => x.Name == "16"))
				this.Clients.Add(new Client
				{
					Name = "16",
					Address = "127.0.0.1:8000",
					PoolId = this.Pools.Single(x => x.Name == "QA").Id
				});
			if (!this.Clients.Any(x => x.Name == "24"))
				this.Clients.Add(new Client
				{
					Name = "24",
					Address = "127.0.0.1:8001",
					PoolId = this.Pools.Single(x => x.Name == "QA").Id
				});
			this.SaveChanges();

			if (!this.Modules.Any(x => x.ClientId == this.Clients.Single(c => c.Name == "16").Id && x.Name == "UI"))
				this.Modules.Add(new Module
				{
					ClientId = this.Clients.Single(c => c.Name == "16").Id,
					Name = "UI",
					Endpoint = "/ui/version",
					Version = "0.7"
				});
			if (!this.Modules.Any(x => x.ClientId == this.Clients.Single(c => c.Name == "24").Id && x.Name == "UI"))
				this.Modules.Add(new Module
				{
					ClientId = this.Clients.Single(c => c.Name == "24").Id,
					Name = "UI",
					Endpoint = "/ui/version",
					Version = "0.6"
				});
			this.SaveChanges();
		}

        public DbSet<Pool> Pools { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Module> Modules { get; set; }
		public DbSet<Package> Packages { get; set; }

        public IEnumerable<Pool> GetPools()
        {
            return this.Pools.ToArray();
        }

		public void AddPool(Pool pool)
		{
			this.Pools.Add(pool);
			this.SaveChanges();
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
				.Include(c => c.Modules)
				//.Join(this.Modules, c => c.Id, m => m.ClientId, (c, m) => c.Modules.Append(m))
				.ToArray();
		}

		public void AddClientToPool(Client cli, int poolId)
		{
			var pool = this.Pools.First(p => p.Id == poolId);
			pool.Clients.Add(cli);
			this.SaveChanges();
		}

		public IEnumerable<Client> GetAllClients()
		{
			return this.Clients;
		}

		public void AddClient(Client cli)
		{
			this.Clients.Add(cli);
			this.SaveChanges();
		}

		public void RemoveClient(Client cli)
		{
			if (cli.Pool != null)
			{
				var pool = this.Pools.First(p => p.Id == cli.Pool.Id);
				pool.Clients.Remove(cli);
			}
			this.Clients.Remove(cli);
			this.SaveChanges();
		}
	}
}
