using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core
{
	public abstract class Command
	{
		public string Name { get; set; }

		public abstract Task<CommandResult> Execute(DeploymentArguments args, CommandResult result, ILogger logger = null);
	}

	public class CommandResult
	{
		public bool Success { get; set; }
		public Command Command { get; set; }
		public DateTime Created { get; set; }

		public CommandResult()
		{
			this.Created = DateTime.UtcNow;
		}
	}
}
