using System;
using System.Collections.Generic;
using System.Text;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IPackageStorage : IDisposable
	{
		IEnumerable<Package> GetPackages();
	}
}
