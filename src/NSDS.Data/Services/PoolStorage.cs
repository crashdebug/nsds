using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class PoolStorage : IPoolStorage
	{
		private readonly ApplicationDbContext context;

		public PoolStorage(ApplicationDbContext context)
		{
			this.context = context;
		}

		public Pool AddPool(Pool pool)
		{
			var dbPool = new PoolDataModel
			{
				Name = pool.Name
			};
			this.context.Pools.Add(dbPool);
			this.context.SaveChanges();
			return dbPool;
		}

		public void RemovePool(int poolId)
		{
			var pool = this.context.Pools.FirstOrDefault(p => p.Id == poolId);
			if (pool != null)
			{
				this.context.Pools.Remove(pool);
				this.context.SaveChanges();
			}
		}

		public void Dispose()
		{
			this.context.Dispose();
		}

		public IEnumerable<Pool> GetPools()
		{
			return this.context.Pools.ToArray();
		}
	}
}
