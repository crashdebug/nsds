using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Pool
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("clients")]
		public IEnumerable<Client> Clients { get; set; }
	}
}
