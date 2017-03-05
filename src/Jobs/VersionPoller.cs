using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApplication.Interfaces;

namespace WebApplication.Jobs
{
    public class VersionPoller : JobBase
	{
        private readonly IClientsService clientService;
        private readonly IEventService eventService;

        public VersionPoller(IClientsService clientService, IEventService eventService, ILogger log) : base(log)
		{
			this.clientService = clientService;
			this.eventService = eventService;
		}

		protected override async void RunOnce()
		{
			foreach (var client in this.clientService.GetAllClients())
			{
				if (!client.Enabled)
				{
					continue;
				}

				using (HttpClient http = new HttpClient())
				{
					http.BaseAddress = new Uri(client.Address);
					foreach (var module in client.Modules)
					{
						try
						{
							var response = await http.GetStringAsync(module.Endpoint);
							var version = JsonConvert.DeserializeObject(response);
							this.eventService.Invoke("VersionReceived", client, module, version);
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
}
