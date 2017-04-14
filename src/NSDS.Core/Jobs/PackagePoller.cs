using System;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;
using NSDS.Core.Services;

namespace NSDS.Core.Jobs
{
	public class PackagePoller : JobBase
	{
		private readonly IPackageStorage packageStorage;
		private readonly IEventService eventService;
		private readonly ConnectionFactory connectionFactory;
		private readonly VersionResolver versionResolver;

		public PackagePoller(IPackageStorage packageStorage, ConnectionFactory connectionFactory, VersionResolver versionResolver, IEventService eventService, ILogger log) : base(log)
		{
			this.packageStorage = packageStorage;
			this.connectionFactory = connectionFactory;
			this.versionResolver = versionResolver;
			this.eventService = eventService;
		}

		protected override async void RunOnce()
		{
			foreach (var package in this.packageStorage.GetPackages())
			{
				try
				{
					var version = await this.versionResolver.GetVersion(package.Endpoint);
					if (version != null && version.CompareTo(package.Version) != 0)
					{
						this.packageStorage.UpdateVersion(package.Name, version);
						this.eventService.Invoke(Constants.Events.PackageVersionReceived, package, version);
					}
				}
				catch (Exception ex)
				{
					this.Log.LogError("Could not get version for package '{0}' @ '{1}':\n{2}", package.Name, package.Endpoint.Url, ex.ToString());
				}
			}
		}
	}
}
