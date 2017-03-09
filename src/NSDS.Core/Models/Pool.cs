using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class Pool
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("clients")]
		public virtual ICollection<Client> Clients { get; set; }
	}
}