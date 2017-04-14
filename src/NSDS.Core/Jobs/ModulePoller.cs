using System;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;
using NSDS.Core.Services;

namespace NSDS.Core.Jobs
{
	public class ModulePoller : JobBase
	{
		private readonly IClientsStorage clientService;
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;
		private readonly VersionResolver versionResolver;

		public ModulePoller(IClientsStorage clientService, ConnectionFactory connectionFactory, VersionResolver versionResolver, IEventService eventService, ILogger log = null) : base(log)
		{
			this.clientService = clientService;
			this.eventService = eventService;
			this.connectionFactory = connectionFactory;
			this.versionResolver = versionResolver;
		}

		protected override async void RunOnce()
		{
			foreach (var client in this.clientService.GetAllClients())
			{
				if (!client.Enabled)
				{
					continue;
				}

				foreach (var module in client.Modules)
				{
					try
					{
						var uri = client.GetEndpointUri(module.Endpoint);
						var version = await this.versionResolver.GetVersion(uri);
						if (version != null && version.CompareTo(module.Version) != 0)
						{ 
							this.clientService.UpdateModuleVersion(client, module, version);
							this.eventService.Invoke(Constants.Events.ModuleVersionReceived, client, module, version);
						}
					}
					catch (Exception ex)
					{
						this.Log.LogError("Could not get version for module '{0}', client '{1}':\n{2}", module.Name, client.Name, ex.ToString());
					}
				}
			}
		}
	}
}
