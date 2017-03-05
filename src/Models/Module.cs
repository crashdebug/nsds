using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WebApplication.Models
{
	public class Module
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		[ForeignKey("ClientId")]
		public virtual Client Client { get; set; }
		[JsonProperty("clientId")]
		public int ClientId { get; set; }

		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }

		[Required]
		[JsonProperty("endpoint")]
		public string Endpoint { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }
	}
}