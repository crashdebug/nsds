using Microsoft.AspNetCore.Mvc;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Api.Controllers
{
	[Route("api/pools")]
	public class PoolsController : Controller
	{
        private readonly IPoolService poolService;

        public PoolsController(IPoolService poolService)
		{
			this.poolService = poolService;
		}

		[Route(""), HttpGet]
		public IActionResult GetPools()
		{
			return Ok(this.poolService.GetPools());
		}

		[Route(""), HttpPost]
		public IActionResult AddPool([FromBody] Pool pool)
		{
			this.poolService.AddPool(pool);
			return Ok();
		}
	}
}
