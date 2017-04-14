using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Data.Services
{
	public class PackageStorage : IPackageStorage
	{
		private readonly IServiceProvider services;

		public PackageStorage(IServiceProvider services)
		{
			this.services = services;
		}

		public void Dispose()
		{
			//this.context.Dispose();
		}

		public Package GetPackage(string name)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return context.Packages
					.Include("Module.Deployment.DeploymentCommands.Command")
					.Include("Deployment.DeploymentCommands.Command")
					.Include("Version")
					.SingleOrDefault(x => x.Name == name)?.ToPackage();
			}
		}

		public IEnumerable<Package> GetPackages()
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return context.Packages
					.Include("Module.Deployment.DeploymentCommands.Command")
					.Include("Deployment.DeploymentCommands.Command")
					.Include("Version")
					.Select(x => x.ToPackage())
					.ToArray();
			}
		}

		public bool UpdateVersion(string name, BaseVersion version)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var package = context.Packages.SingleOrDefault(x => x.Name == name);
				if (package == null)
				{
					return false;
				}
				var existing = context.Versions.SingleOrDefault(x => x.Version == version.Version);
				if (existing == null)
				{
					existing = context.Versions.Add(version).Entity;
				}
				package.Version = existing;
				context.SaveChanges();
				return true;
			}
		}
	}
}
