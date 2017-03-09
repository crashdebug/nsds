using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Client
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		[Required]
		public string Name { get; set; }

		[JsonProperty("address")]
		[Required]
		public string Address { get; set; }

		[JsonIgnore]
		[ForeignKey("PoolId")]
		public virtual Pool Pool { get; set; }
		[JsonProperty("poolId")]
		public int PoolId { get; set; }

		[JsonProperty("modules")]
		public virtual IEnumerable<Module> Modules { get; set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; set; }
    }
}