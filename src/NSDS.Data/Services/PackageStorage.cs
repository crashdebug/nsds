using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Data.Services
{
	public class PackageStorage : IPackageStorage
	{
		private readonly ApplicationDbContext context;

		public PackageStorage(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void Dispose()
		{
			this.context.Dispose();
		}

		public Package GetPackage(string name)
		{
			return this.context.Packages
				.Include("Module.Deployment.DeploymentCommands.Command")
				.Include("Deployment.DeploymentCommands.Command")
				.Include("Version")
				.SingleOrDefault(x => x.Name == name)?.ToPackage();
		}

		public IEnumerable<Package> GetPackages()
		{
			return this.context.Packages
				.Include("Module.Deployment.DeploymentCommands.Command")
				.Include("Deployment.DeploymentCommands.Command")
				.Include("Version")
				.Select(x => x.ToPackage())
				.AsEnumerable();
		}

		public bool UpdateVersion(string name, BaseVersion version)
		{
			var package = this.context.Packages.SingleOrDefault(x => x.Name == name);
			if (package == null)
			{
				return false;
			}
			var existing = this.context.Versions.SingleOrDefault(x => x.Version == version.Version);
			if (existing == null)
			{
				existing = this.context.Versions.Add(version).Entity;
			}
			package.Version = existing;
			this.context.SaveChanges();
			return true;
		}
	}
}
