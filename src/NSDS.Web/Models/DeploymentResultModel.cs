using System.Collections;
using Newtonsoft.Json;
using NSDS.Core.Interfaces;

namespace NSDS.Web.Models
{
	public class DeploymentResultModel
	{
		//[JsonProperty("output")]
		[JsonIgnore]
		public IEnumerable Output { get; set; }
		[JsonProperty("result")]
		public DeploymentResult Result { get; set; }
	}
}
