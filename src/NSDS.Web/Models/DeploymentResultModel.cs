using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Interfaces;

namespace NSDS.Web.Models
{
	public class DeploymentResultModel
	{
		[JsonProperty("output")]
		public IEnumerable Output { get; set; }
		[JsonProperty("results")]
		public DeploymentResult Results { get; set; }
		[JsonProperty("version")]
		public string Version { get; internal set; }
	}
}
