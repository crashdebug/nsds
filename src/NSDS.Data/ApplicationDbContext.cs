using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core;
using NSDS.Core.Commands;
using NSDS.Core.Models;
using NSDS.Data.Configuration;
using NSDS.Data.Models;

namespace NSDS.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions context) :
			base(context)
		{
			AutoMapperConfig.Configure();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			builder.Entity<BaseVersion>().ToTable("Versions").HasKey(v => v.Version);
			builder.Entity<DateVersion>();
			builder.Entity<NumericVersion>();

			builder.Entity<ClientModuleDataModel>().HasKey(x => new { x.ClientId, x.ModuleId });
			builder.Entity<ClientModuleDataModel>().HasOne(x => x.Module).WithMany().HasForeignKey("ModuleId");
			builder.Entity<CommandDataModel>().ToTable("Commands").HasKey(x => x.Name);
			builder.Entity<DeploymentDataModel>().Property(x => x.Name).IsRequired(true);
			builder.Entity<DeploymentCommandsDataModel>().HasKey(x => new { x.CommandName, x.DeploymentId });
		}

		public void Seed()
		{
			this.Database.Migrate();

			var module = this.Modules.SingleOrDefault(x => x.Name == "www");
			if (module == null)
			{
				module = this.Modules.Add(new ModuleDataModel
				{
					Name = "www",
					Endpoint = "http://{0}/api/version",
					PathQuery = "/root/version",
				}).Entity;
				this.SaveChanges();
			}

			var client = this.Clients.SingleOrDefault(x => x.Name == "nsds");
			if (client == null)
			{
				client = new ClientDataModel
				{
					Name = "nsds",
					Address = "localhost:3000",
					Enabled = true,
					Created = DateTime.UtcNow,
				};
				client.ClientModules = new List<ClientModuleDataModel> { new ClientModuleDataModel { Client = client, Module = module } };
				this.Clients.Add(client);
				this.SaveChanges();
			}

			var build = this.Deployments.SingleOrDefault(x => x.Name == "build");
			if (build == null)
			{
				build = this.Deployments.Add(new DeploymentDataModel
				{
					Name = "build",
					Module = module,
					Created = DateTime.UtcNow,
					Commands = new List<Command> { new ShellCommand("dotnet", @"pack NSDS.Web.csproj -o wwwroot\pkg") }
				}).Entity;
				this.SaveChanges();
			}
			var publish = this.Deployments.SingleOrDefault(x => x.Name == "publish");
			if (publish == null)
			{
				publish = this.Deployments.Add(new DeploymentDataModel
				{
					Name = "publish",
					Module = module,
					Created = DateTime.UtcNow,
					Commands = new List<Command> {  new ShellCommand("dotnet", @"publish NSDS.Web.csproj -o ..\..\deploy")},
				}).Entity;
				this.SaveChanges();
			}
			module.Deployment = publish;
			this.SaveChanges();

			var package = this.Packages.SingleOrDefault(x => x.Module == module);
			if (package == null)
			{
				package = this.Packages.Add(new PackageDataModel
				{
					Name = "build www",
					Created = DateTime.UtcNow,
					Module = module,
					Deployment = build,
					Url = "http://localhost:3000/api/version/latest/package",
					PathQuery = "/root/version"
				}).Entity;
				this.SaveChanges();
			}

			module.Package = package;
			this.SaveChanges();

			/*var uiVersionOld = this.Versions.FirstOrDefault(x => x.Version == "2017-02-14 00:00:00");
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

			var packageVersion = this.Versions.FirstOrDefault(x => x.Version == "2017-03-15 00:00:00");
			if (packageVersion == null)
			{
				packageVersion = new DateVersion("2017-03-15 00:00:00");
				this.Versions.Add(packageVersion);
				this.SaveChanges();
			};

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

			var uiDeployment = this.Deployments.FirstOrDefault(x => x.Name == "deploy_ui");
			if (uiDeployment == null)
			{
				uiDeployment = new DeploymentDataModel
				{
					Name = "deploy_ui",
					Created = DateTime.UtcNow,
				};
				this.Deployments.Add(uiDeployment);
				this.SaveChanges();
			}

			var uiModule = this.Modules.FirstOrDefault(x => x.Name == "UI");
			if (uiModule == null)
			{
				uiModule = new ModuleDataModel
				{
					Name = "UI",
					Endpoint = "/ui/version",
					Deployment = uiDeployment,
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
					PoolId = this.Pools.Single(x => x.Name == "QA").Id,
					Created = DateTime.UtcNow,
				};
				this.Clients.Add(client);
				this.ClientModules.Add(new ClientModuleDataModel
				{
					Client = client,
					Module = uiModule,
					Version = uiVersionOld,
//					Created = DateTime.UtcNow
				});
			}
			if (!this.Clients.Any(x => x.Name == "24"))
			{
				var client = new ClientDataModel
				{
					Name = "24",
					Address = "127.0.0.1:8001",
					PoolId = this.Pools.Single(x => x.Name == "QA").Id,
					Created = DateTime.UtcNow,
				};
				this.Clients.Add(client);
				this.ClientModules.Add(new ClientModuleDataModel
				{
					Client = client,
					Module = uiModule,
					Version = uiVersionNew,
//					Created = DateTime.UtcNow
				});
			}
			this.SaveChanges();

			var uiBuild = this.Deployments.FirstOrDefault(x => x.Name == "build_ui");
			if (uiBuild == null)
			{
				uiBuild = new DeploymentDataModel
				{
					Name = "build_ui",
					Created = DateTime.UtcNow,
				};
				this.Deployments.Add(uiBuild);
				this.SaveChanges();
			}

			var package = this.Packages.FirstOrDefault(x => x.Version == packageVersion);
			if (package == null)
			{
				package = new PackageDataModel
				{
					Created = DateTime.UtcNow,
					Version = packageVersion,
					Module = uiModule,
					Deployment = uiBuild,
				};
				this.Packages.Add(package);
				this.SaveChanges();
			}*/
		}

		public DbSet<PoolDataModel> Pools { get; set; }
		public DbSet<ClientDataModel> Clients { get; set; }
		public DbSet<ModuleDataModel> Modules { get; set; }
		public DbSet<ClientModuleDataModel> ClientModules { get; set; }
		public DbSet<PackageDataModel> Packages { get; set; }
		public DbSet<BaseVersion> Versions { get; set; }
		public DbSet<DeploymentDataModel> Deployments { get; set; }
		public DbSet<DeploymentCommandsDataModel> DeploymentCommands { get; set; }
		public DbSet<CommandDataModel> Commands { get; set; }
	}
}
