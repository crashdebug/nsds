using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

		public Package GetPackage(int packageId)
		{
			return this.context.Packages
				.Include(x => x.Module).ThenInclude(x => x.Deployment).ThenInclude(x => x.DeploymentCommands)
				.Include(x => x.Deployment).ThenInclude(x => x.DeploymentCommands)
				.SingleOrDefault(x => x.Id == packageId)?.ToPackage();
		}

		public IEnumerable<Package> GetPackages()
		{
			return this.context.Packages
				.Include(x => x.Module).ThenInclude(x => x.Deployment).ThenInclude(x => x.DeploymentCommands)
				.Include(x => x.Deployment).ThenInclude(x => x.DeploymentCommands)
				.Select(x => x.ToPackage())
				.AsEnumerable();
		}
	}
}
