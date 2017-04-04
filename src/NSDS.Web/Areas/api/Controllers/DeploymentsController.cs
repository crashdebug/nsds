using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSDS.Core;
using NSDS.Core.Extensions;
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

		[Produces("text/plain")]
		[Route("package/{name}/{packageName}"), HttpGet]
		public Task ExecutePackageDeploymentAsync(string name, string packageName)
		{
			return Task.Run(async () =>
			{
				var deployment = this.deploymentStorage.GetDeployment(name);
				if (deployment == null)
				{
					this.Error(400, $"Deployment with name '{name}' not found");
					return;
				}
				var package = this.packageStorage.GetPackage(packageName);
				if (package == null)
				{
					this.Error(400, $"Package with id {packageName} not found");
					return;
				}
				var logger = new ResponseLogger(this.Response.Body);
				await this.deploymentService.Deploy(deployment, new DeploymentArguments
				{
					Package = package,
					Environment = new Dictionary<string, object>
					{
						{ "workingDir", Directory.GetCurrentDirectory() },
						{ "date", DateTime.UtcNow },
						{ "deployment", name },
					}
				}, logger);
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
							 return Ok();
						 default:
							 return BadRequest();
					 }
				 });
			}
			return Ok();
		}

		private void Error(int statusCode, string message)
		{
			this.Response.StatusCode = statusCode;
			this.Response.Body.Write(Encoding.UTF8.GetBytes(message));
		}
	}

	class ResponseLogger : ILogger
	{
		private Stack<Scope> stack = new Stack<Scope>();
		private readonly StreamWriter stream;

		public ResponseLogger(Stream body)
		{
			this.stream = new StreamWriter(body);
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			Scope scope = new Scope(state);
			this.stack.Push(scope);
			return scope;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			this.stream.WriteLine(formatter(state, exception));
		}

		private class Scope : IDisposable
		{
			private object state;

			public Scope(object state)
			{
				this.state = state;
			}

			public void Dispose()
			{
			}
		}
	}
}