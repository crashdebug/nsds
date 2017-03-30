using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Deployment
    {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("commands")]
		public ICollection<Command> Commands { get; set; }
	}
}
