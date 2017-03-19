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
    [Route("api/packages")]
    public class PackagesController : Controller
    {
		private readonly IPackageStorage packageStorage;

		public PackagesController(IPackageStorage packageStorage)
		{
			this.packageStorage = packageStorage;
		}

		[Route(""), HttpGet]
		public IActionResult GetPackages()
		{
			return Ok(this.packageStorage.GetPackages());
		}
    }
}