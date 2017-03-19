using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public abstract class Deployment
    {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("commands")]
		public abstract ICollection<Command> Commands { get; set; }
	}
}
