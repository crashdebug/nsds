using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IPackageStorage : IDisposable
	{
		IEnumerable<Package> GetPackages();
		Package GetPackage(string packageName);
	}
}
