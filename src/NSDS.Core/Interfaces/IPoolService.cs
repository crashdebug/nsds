using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IPoolService : IDisposable
	{
		IEnumerable<Pool> GetPools();
		Pool AddPool(Pool pool);
	}
}