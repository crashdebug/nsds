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

		public void AddPool(Pool pool)
		{
			this.context.Pools.Add(new PoolDataModel
			{
				Name = pool.Name,
				Created = DateTime.UtcNow,
			});
			this.context.SaveChanges();
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
			return this.context.Pools.Select(x => x.ToPool()).ToArray();
		}
	}
}
