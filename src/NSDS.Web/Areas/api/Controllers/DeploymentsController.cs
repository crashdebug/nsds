using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSDS.Core;
using NSDS.Core.Interfaces;

namespace NSDS.Web.Areas.api.Controllers
{
	[Produces("application/json")]
    [Route("api/deploy")]
    public class DeploymentsController : Controller
    {
		private readonly IDeploymentStorage deploymentStorage;
		private readonly IClientsStorage clientStorage;
		private readonly IModuleStorage moduleStorage;
		private readonly IDeploymentService deploymentService;
		private readonly IPackageStorage packageStorage;

		public DeploymentsController(IDeploymentStorage deploymentStorage, IClientsStorage clientStorage, IModuleStorage moduleStorage, IDeploymentService deploymentService, IPackageStorage packageStorage)
		{
			this.deploymentStorage = deploymentStorage;
			this.clientStorage = clientStorage;
			this.moduleStorage = moduleStorage;
			this.deploymentService = deploymentService;
			this.packageStorage = packageStorage;
		}

		[Route(""), HttpGet]
		public IActionResult GetDeployments()
		{
			return Ok(this.deploymentStorage.GetDeployments());
		}

		[Route("package/{name}/{packageId}"), HttpGet]
		public Task<IActionResult> ExecutePackageDeploymentAsync(string name, int packageId)
		{
			return Task.Run<IActionResult>(() =>
			{
				var deployment = this.deploymentStorage.GetDeployment(name);
				if (deployment == null)
				{
					return BadRequest($"Deployment with name '{name}' not found");
				}
				var package = this.packageStorage.GetPackage(packageId);
				if (package == null)
				{
					return BadRequest($"Package with id {packageId} not found");
				}
				var task = this.deploymentService.Deploy(deployment, new DeploymentArguments
				{
					Package = package,
					Environment = new Dictionary<string, object>
					{
						{ "workingDir", Directory.GetCurrentDirectory() },
						{ "date", DateTime.UtcNow },
						{ "deployment", name },
					}
				});
				task.Wait();
				return Ok();
			});
		}

		[Route("module/{name}"), HttpGet]
		public async Task<IActionResult> ExecuteModuleDeploymentAsync(string name, int moduleId, int[] clientIds)
		{
			var deployment = this.deploymentStorage.GetDeployment(name);
			if (deployment == null)
			{
				return BadRequest($"Deployment with name '{name}' not found");
			}
			var module = this.moduleStorage.GetModule(moduleId);
			if (module == null)
			{
				return BadRequest($"Module with id {moduleId} not found");
			}
			List<Task<IEnumerable<CommandResult>>> tasks = new List<Task<IEnumerable<CommandResult>>>();
			foreach (var id in clientIds)
			{
				var client = this.clientStorage.GetClient(id);
				if (client != null)
				{
					var task = this.deploymentService.Deploy(deployment, new DeploymentArguments
					{
						Client = client,
						Module = module,
						Environment = new Dictionary<string, object>
						{
							{ "workingDir", Directory.GetCurrentDirectory() },
							{ "date", DateTime.UtcNow },
							{ "deployment", name },
						}
					});
					tasks.Add(task);
				}
			}
			if (tasks.Count > 0)
			{
				return await Task.WhenAll(tasks).ContinueWith<IActionResult>(t =>
				 {
					 switch (t.Status)
					 {
						 case TaskStatus.RanToCompletion:
							 return Ok(t.Result);
						 default:
							 return BadRequest();
					 }
				 });
			}
			return Ok();
		}
	}
}