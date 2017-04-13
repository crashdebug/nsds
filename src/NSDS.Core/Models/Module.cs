using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Module
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("endpoint")]
		public VersionResource Endpoint { get; set; }

		[JsonProperty("version")]
		public BaseVersion Version { get; set; }

		[JsonProperty("deployment")]
		public Deployment Deployment { get; set; }
	}
}