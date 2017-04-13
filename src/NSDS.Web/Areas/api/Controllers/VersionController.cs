using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace NSDS.Web.Areas.api.Controllers
{
	[Produces("application/json")]
	[Route("api/version")]
	public class VersionController : Controller
	{
		private readonly IHostingEnvironment hostingEnv;
		private static Regex versionPattern = new Regex(@"(?<version>\d+(\.\d+){1,3})");

		public VersionController(IHostingEnvironment hostingEnv)
		{
			this.hostingEnv = hostingEnv;
		}

		[Route("packages")]
		public IActionResult Packages()
		{
			return Ok(this.GetPackages().OrderBy(x => x.created));
		}

		[Route("latest/package"), HttpGet]
		public IActionResult LatestPackage()
		{
			return Ok(this.GetPackages().OrderByDescending(x => x.created).FirstOrDefault());
		}

		private IEnumerable<dynamic> GetPackages()
		{
			var path = Path.Combine(this.hostingEnv.WebRootPath, "pkg");
			var dir = new DirectoryInfo(path);
			if (!dir.Exists)
			{
				return new object[0];
			}

			return dir.GetFiles().Select(x => new
			{
				m = versionPattern.Match(x.Name),
				f = x,
			})
			.Where(x => x.m.Success)
			.Select(x => new
			{
				version = x.m.Groups["version"].Value,
				created = x.f.CreationTimeUtc,
				name = x.f.Name,
			});
		}
	}
}