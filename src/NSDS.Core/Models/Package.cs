using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NSDS.Core.Commands;

namespace NSDS.Core.Models
{
	public class Package
	{
		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("version")]
		public BaseVersion Version { get; set; }

		[NotMapped]
		[JsonProperty("commands")]
		public virtual ICollection<Command> Commands { get; set; }
	}
}
