using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core.Services
{
	public class DeploymentService : IDeploymentService
	{
		private readonly IEventService eventService;
		private readonly IClientsStorage clientService;
		private readonly ConnectionFactory connectionFactory;
		private readonly VersionResolver versionConsumer;

		public DeploymentService(IEventService eventService, ConnectionFactory connectionFactory, VersionResolver versionConsumer, IClientsStorage clientService = null)
		{
			this.eventService = eventService;
			this.clientService = clientService;
			this.connectionFactory = connectionFactory;
			this.versionConsumer = versionConsumer;
		}

		public async Task<DeploymentResult> Deploy(Package package, IDictionary<string, object> environment = null, ILogger logger = null)
		{
			var result = await this.Deploy<PackageDeploymentResult>(package.Deployment, new DeploymentArguments
			{
				Package = package,
				Environment = environment,
			}, logger);
			result.Package = package;
			result.Version = await GetVersion(package.Endpoint, result);
			if (result.Version != null && result.Version.CompareTo(package.Version) != 0)
			{
				this.eventService.Invoke(Constants.Events.PackageVersionReceived, package, result.Version);
			}
			return result;
		}

		public async Task<DeploymentResult> Deploy(Client client, ClientModule module, IDictionary<string, object> environment = null, ILogger logger = null)
		{
			var result = await this.Deploy<ModuleDeploymentResult>(module.Module.Deployment, new DeploymentArguments
			{
				Client = client,
				Module = module.Module,
				Environment = environment,
			}, logger);
			result.Client = client;
			result.Module = module.Module;
			result.ClientModule = this.clientService?.GetClientModule(client.Name, module.Module.Name);
			result.Version = await GetVersion(client.GetEndpointUri(module.Module.Endpoint), result);
			if (result.Version != null && result.Version.CompareTo(module.Version) != 0)
			{
				this.eventService.Invoke(Constants.Events.PackageVersionReceived, client, module, result.Version);
			}
			return result;
		}

		private async Task<BaseVersion> GetVersion(VersionResource resource, DeploymentResult result)
		{
			if (!result.Success)
			{
				return null;
			}
			return await this.versionConsumer.GetVersion(resource);
		}

		private async Task<T> Deploy<T>(Deployment deployment, DeploymentArguments args, ILogger logger = null) where T : DeploymentResult, new()
		{
			var result = new T();
			foreach (var command in deployment.Commands)
			{
				var cmdResult = new CommandResult { Command = command };
				using (var scope = logger?.BeginScope(cmdResult))
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
