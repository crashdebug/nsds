using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core.Interfaces;

namespace NSDS.Core.Jobs
{
	public class VersionPoller : JobBase
	{
		private readonly IClientsStorage clientService;
		private readonly IModuleStorage moduleService;
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;

		public VersionPoller(IClientsStorage clientService, IModuleStorage moduleService, ConnectionFactory connectionFactory, IEventService eventService, ILogger log = null) : base(log)
		{
			this.clientService = clientService;
			this.moduleService = moduleService;
			this.eventService = eventService;
			this.connectionFactory = connectionFactory;
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
						var conn = this.connectionFactory.CreateConnection(new Uri(uri.Url));
						using (var stream = new StreamReader(await conn.GetStream()))
						{
							var version = JsonConvert.DeserializeObject(stream.ReadToEnd());
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
