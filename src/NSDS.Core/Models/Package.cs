using System;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public abstract class Package
	{
		//[JsonProperty("module")]
		//public virtual string ModuleName { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("version")]
		public BaseVersion Version { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		//[JsonIgnore]
		//public Deployment Deployment { get; set; }
	}
}
