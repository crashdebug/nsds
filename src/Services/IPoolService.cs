using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
	public interface IPoolService
	{
		IEnumerable<Pool> GetPools();
		void AddPool(Pool pool);
	}
}