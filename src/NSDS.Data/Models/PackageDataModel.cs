using System;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PackageDataModel : Package
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		public string VersionId { get; set; }

		[JsonProperty("module")]
		public ModuleDataModel Module { get; set; }

		[JsonProperty("moduleId")]
		public int ModuleId { get; set; }

		[JsonProperty("deploymentId")]
		public int? DeploymentId { get; set; }

		[JsonProperty("deployment")]
		public DeploymentDataModel Deployment { get; set; }
	}
}
