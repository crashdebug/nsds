using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public abstract class Client
	{
		[JsonProperty("name")]
		[Required]
		public string Name { get; set; }

		[JsonProperty("address")]
		[Required]
		public string Address { get; set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		public Uri GetEndpointUri(string endpoint)
		{
			return new Uri(string.Format(endpoint, this.Address));
		}
	}
}