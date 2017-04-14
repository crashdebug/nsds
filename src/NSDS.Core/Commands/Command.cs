using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core.Interfaces;

namespace NSDS.Core
{
	public abstract class Command
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		public abstract Task Execute(DeploymentArguments args, CommandResult result, ILogger logger = null);
	}

	public class CommandResult
	{
		[JsonProperty("success")]
		public bool Success { get; set; }
		[JsonProperty("command")]
		public Command Command { get; set; }
		[JsonProperty("created")]
		public DateTime Created { get; set; }
		[JsonProperty("error")]
		public string Error { get; set; }

		public CommandResult()
		{
			this.Created = DateTime.UtcNow;
		}
	}
}
