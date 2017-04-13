using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Extensions;
using NSDS.Core.Interfaces;
using NSDS.Web.Models;

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
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;

		public DeploymentsController(IDeploymentStorage deploymentStorage, IClientsStorage clientStorage, IModuleStorage moduleStorage, IDeploymentService deploymentService, IPackageStorage packageStorage, IEventService eventService, ConnectionFactory connectionFactory)
		{
			this.deploymentStorage = deploymentStorage;
			this.clientStorage = clientStorage;
			this.moduleStorage = moduleStorage;
			this.deploymentService = deploymentService;
			this.packageStorage = packageStorage;
			this.eventService = eventService;
			this.connectionFactory = connectionFactory;
		}

		[Route(""), HttpGet]
		public IActionResult GetDeployments()
		{
			return Ok(this.deploymentStorage.GetDeployments());
		}

		[Produces("text/plain")]
		[Route("package/{name}"), HttpGet]
		public Task DeployPackage(string name)
		{
			return Task.Run(async () =>
			{
				/*var deployment = this.deploymentStorage.GetDeployment(name);
				if (deployment == null)
				{
					this.Error(400, $"Deployment with name '{name}' not found");
					return;
				}*/
				var package = this.packageStorage.GetPackage(name);
				if (package == null)
				{
					this.Error(400, $"Package with name '{name}' not found");
					return;
				}

				var logger = new MemoryLogger();

				var result = await this.deploymentService.Deploy(package, new Dictionary<string, object>
					{
						{ "workingDir", Directory.GetCurrentDirectory() },
						{ "date", DateTime.UtcNow },
						{ "deployment", name },
					}, logger);

				if (result.Version != null && result.Version.CompareTo(package.Version) != 0)
				{
					this.packageStorage.UpdateVersion(package.Name, result.Version);
				}

				var model = new DeploymentResultModel
				{
					Output = logger.ToArray(),
					Results = result,
				};

				using (StreamWriter writer = new StreamWriter(this.Response.Body, Encoding.UTF8, 1024, true) { AutoFlush = true })
				{
					writer.Write(JsonConvert.SerializeObject(model));
				}
			});
		}

		[Route("module/{name}"), HttpGet]
		public async Task<IActionResult> ExecuteModuleDeploymentAsync(string name, int[] clientIds)
		{
			/*var deployment = this.deploymentStorage.GetDeployment(name);
			if (deployment == null)
			{
				return BadRequest($"Deployment with name '{name}' not found");
			}*/
			var module = this.moduleStorage.GetModule(name);
			if (module == null)
			{
				return BadRequest($"Module with name '{name}' not found");
			}
			List<Task<DeploymentResult>> tasks = new List<Task<DeploymentResult>>();
			foreach (var id in clientIds)
			{
				var client = this.clientStorage.GetClient(id);
				if (client != null)
				{
					var task = this.deploymentService.Deploy(client, module, new Dictionary<string, object>
						{
							{ "workingDir", Directory.GetCurrentDirectory() },
							{ "date", DateTime.UtcNow },
							{ "client", client },
							{ "module", module },
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

	class MemoryLogger : ILogger
	{
		private class LogEntry
		{
			[JsonProperty("message")]
			public string Message { get; internal set; }
			[JsonProperty("level")]
			public LogLevel Level { get; internal set; }
			[JsonProperty("eventId")]
			public EventId EventId { get; internal set; }
		}

		private class Scope : IDisposable
		{
			[JsonProperty("data")]
			public readonly List<LogEntry> Data = new List<LogEntry>();
			[JsonProperty("state")]
			public readonly object State;

			public Scope(object state)
			{
				this.State = state;
			}

			public void Dispose()
			{
			}
		}

		private readonly List<Scope> Commands = new List<Scope>();
		private Scope currentScope;

		public IDisposable BeginScope<TState>(TState state)
		{
			this.currentScope = new Scope(state);
			this.Commands.Add(this.currentScope);
			return this.currentScope;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			this.currentScope.Data.Add(new LogEntry
			{
				Message = formatter(state, exception),
				Level = logLevel,
				EventId = eventId,
			});
		}

		public IEnumerable<object> ToArray()
		{
			return this.Commands;
		}
	}
}