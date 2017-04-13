using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core.Interfaces;

namespace NSDS.Core.Jobs
{
	public class PackagePoller : JobBase
	{
		private readonly IPackageStorage packageStorage;
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;

		public PackagePoller(IPackageStorage packageStorage, ConnectionFactory connectionFactory, IEventService eventService, ILogger log) : base(log)
		{
			this.packageStorage = packageStorage;
			this.connectionFactory = connectionFactory;
			this.eventService = eventService;
		}

		protected override async void RunOnce()
		{
			foreach (var package in this.packageStorage.GetPackages())
			{
				try
				{
					var conn = this.connectionFactory.CreateConnection(new Uri(package.Endpoint.Url));
					using (var stream = new StreamReader(await conn.GetStream()))
					{
						var version = JsonConvert.DeserializeObject(stream.ReadToEnd());
						this.eventService.Invoke(Constants.Events.PackageVersionReceived, package, version);
					}
				}
				catch (Exception ex)
				{
					this.Log.LogError("Could not get version for package '{0}':\n{1}", package.Endpoint.Url, ex.ToString());
				}
			}
		}
	}
}
