using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public sealed class ModuleDataModel : Module
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		public string VersionId { get; set; }

		[JsonProperty("deploymentId")]
		public int DeploymentId { get; set; }

		[JsonProperty("deployment")]
		public DeploymentDataModel Deployment { get; set; }
	}
}
