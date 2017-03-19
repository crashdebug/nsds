using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSDS.Core.Interfaces;

namespace NSDS.Web.Areas.api.Controllers
{
    [Produces("application/json")]
    [Route("api/deployments")]
    public class DeploymentsController : Controller
    {
		private readonly IDeploymentStorage deploymentStorage;

		public DeploymentsController(IDeploymentStorage deploymentStorage)
		{
			this.deploymentStorage = deploymentStorage;
		}

		[Route(""), HttpGet]
		public IActionResult GetDeployments()
		{
			return Ok(this.deploymentStorage.GetDeployments());
		}
    }
}