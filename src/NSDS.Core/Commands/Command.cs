using System;
using Microsoft.Extensions.Logging;
using NSDS.Core.Models;

namespace NSDS.Core
{
	public abstract class Command
	{
		public string Name { get; set; }

		public abstract CommandResult Execute(Client client, Module module, ILogger logger = null);
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
