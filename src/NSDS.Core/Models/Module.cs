using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public abstract class Module
	{
		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }

		[Required]
		[JsonProperty("endpoint")]
		public string Endpoint { get; set; }

		[JsonProperty("version")]
		public BaseVersion Version { get; set; }

		//[JsonIgnore]
		//public Deployment Deployment { get; set; }
	}
}