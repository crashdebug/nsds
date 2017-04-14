using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Services
{
	public class PoolStorage : IPoolStorage
	{
		private readonly IServiceProvider services;

		public PoolStorage(IServiceProvider services)
		{
			this.services = services;
		}

		public void AddPool(Pool pool)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				context.Pools.Add(new PoolDataModel
				{
					Name = pool.Name,
					Created = DateTime.UtcNow,
				});
				context.SaveChanges();
			}
		}

		public void RemovePool(int poolId)
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				var pool = context.Pools.FirstOrDefault(p => p.Id == poolId);
				if (pool != null)
				{
					context.Pools.Remove(pool);
					context.SaveChanges();
				}
			}
		}

		public void Dispose()
		{
			//this.context.Dispose();
		}

		public IEnumerable<Pool> GetPools()
		{
			using (var context = this.services.GetService<ApplicationDbContext>())
			{
				return context.Pools.Select(x => x.ToPool()).ToArray();
			}
		}
	}
}
