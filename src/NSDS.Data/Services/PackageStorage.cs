using System;
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

		public IEnumerable<Package> GetPackages()
		{
			return this.context.Packages.Include(x => x.Module).Include(x => x.Deployment).ThenInclude(x => x.DeploymentCommands).AsEnumerable();
		}
	}
}
