using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using WebApplication.Commands;

namespace WebApplication.Models
{
	public class Package
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[NotMapped]
		[JsonProperty("commands")]
		public virtual IEnumerable<Command> Commands { get; set; }
	}
}
