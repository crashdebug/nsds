using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core.Services
{
	public class DeploymentService : IDeploymentService
	{
		private readonly IEventService eventService;

		public DeploymentService(IEventService eventService)
		{
			this.eventService = eventService;
		}

		public Task<IEnumerable<CommandResult>> Deploy(Deployment deployment, DeploymentArguments args, ILogger logger = null)
		{
			return Task.Run(async () =>
			{
				List<CommandResult> results = new List<CommandResult>();
				foreach (var command in deployment.Commands)
				{
					var result = new CommandResult { Command = command };
					await command.Execute(args, result, logger);
					results.Add(result);
					if (!result.Success)
					{
						break;
					}
				}
				return results.AsEnumerable();
			});
		}
	}
}
