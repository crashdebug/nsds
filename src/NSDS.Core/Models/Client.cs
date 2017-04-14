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
		public IEnumerable<ClientModule> Modules { get; set; }

		public VersionResource GetEndpointUri(VersionResource endpoint)
		{
			return new VersionResource
			{
				Url = string.Format(endpoint.Url, this.Address),
				PathQuery = endpoint.PathQuery,
			};
		}
	}
}