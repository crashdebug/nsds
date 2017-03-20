using Microsoft.AspNetCore.Mvc;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Web.Api.Controllers
{
	[Route("api/clients")]
	public class ClientsController : Controller
	{
        private readonly IClientsStorage clientService;

        public ClientsController(IClientsStorage clientService)
		{
			this.clientService = clientService;
		}

		[Route("pool/{poolId}")]
		public IActionResult ClientsInPool(int poolId)
		{
			return Ok(this.clientService.GetClientsInPool(poolId));
		}

		[Route(""), HttpPost]
		public IActionResult AddClient([FromBody] Client client)
		{
			this.clientService.AddClient(client);
			return Ok();
		}
	}
}
