using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Client
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("address")]
		public string Address { get; set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("modules")]
		public IEnumerable<Module> Modules { get; set; }

		public Uri GetEndpointUri(string endpoint)
		{
			return new Uri(string.Format(endpoint, this.Address));
		}
	}
}