using System;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Package
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("version")]
		public BaseVersion Version { get; set; }

		[JsonProperty("versionResource")]
		public VersionResource Endpoint { get; set; }

		[JsonProperty("module")]
		public Module Module { get; set; }

		[JsonProperty("deployment")]
		public Deployment Deployment { get; set; }

		public Package()
		{
			this.Created = DateTime.UtcNow;
		}
	}
}
