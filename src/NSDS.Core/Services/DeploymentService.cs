using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core.Services
{
	public class DeploymentService : IDeploymentService
	{
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;
		private readonly VersionResolver versionConsumer;

		public DeploymentService(IEventService eventService, ConnectionFactory connectionFactory, VersionResolver versionConsumer)
		{
			this.eventService = eventService;
			this.connectionFactory = connectionFactory;
			this.versionConsumer = versionConsumer;
		}

		public async Task<DeploymentResult> Deploy(Package package, IDictionary<string, object> environment = null, ILogger logger = null)
		{
			var result = await this.Deploy(package.Deployment, new DeploymentArguments
			{
				Package = package,
				Environment = environment,
			}, logger);
			result.Version = await GetVersion(package.Endpoint, result);
			if (result.Version != null && result.Version.CompareTo(package.Version) != 0)
			{
				this.eventService.Invoke(Constants.Events.PackageVersionReceived, package, result.Version);
			}
			return result;
		}

		public async Task<DeploymentResult> Deploy(Client client, Module module, IDictionary<string, object> environment = null, ILogger logger = null)
		{
			var result = await this.Deploy(module.Deployment, new DeploymentArguments
			{
				Client = client,
				Module = module,
				Environment = environment,
			}, logger);
			result.Version = await GetVersion(client.GetEndpointUri(module.Endpoint), result);
			if (result.Version != null && result.Version.CompareTo(module.Version) != 0)
			{
				this.eventService.Invoke(Constants.Events.PackageVersionReceived, client, module, result.Version);
			}
			return result;
		}

		private async Task<BaseVersion> GetVersion(VersionResource resource, DeploymentResult result)
		{
			if (!result.Success || string.IsNullOrWhiteSpace(resource.Url))
			{
				return null;
			}
			using (var conn = this.connectionFactory.CreateConnection(new Uri(resource.Url)))
			{
				using (var reader = new StreamReader(await conn.GetStream()))
				{
					return this.versionConsumer.CheckVersion(JsonConvert.DeserializeXmlNode(await reader.ReadToEndAsync(), "root"), resource.PathQuery);
				}
			}
		}

		private async Task<DeploymentResult> Deploy(Deployment deployment, DeploymentArguments args, ILogger logger = null)
		{
			var result = new DeploymentResult();
			foreach (var command in deployment.Commands)
			{
				var cmdResult = new CommandResult { Command = command };
				using (var scope = logger?.BeginScope(result))
				{
					await command.Execute(args, cmdResult, logger);
				}
				result.Add(cmdResult);
				if (!cmdResult.Success)
				{
					break;
				}
			}
			return result;
		}
	}
}
