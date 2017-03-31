using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IPoolStorage : IDisposable
	{
		IEnumerable<Pool> GetPools();
		void AddPool(Pool pool);
	}
}