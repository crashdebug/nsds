using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public abstract class Pool
	{
		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}