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
		public Task<IEnumerable<CommandResult>> DeployModule(Client client, Module module, Deployment deployment, ILogger logger = null)
		{
			return Task.Run(() =>
			{
				List<CommandResult> results = new List<CommandResult>();
				foreach (var command in deployment.Commands)
				{
					var result = command.Execute(client, module, logger);
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
