using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core.Services
{
	public class DeploymentService : IDeploymentService
	{
		public Task<IEnumerable<CommandResult>> DeployModule(Client client, Module module, Deployment deployment)
		{
			return Task.Run(() =>
			{
				List<CommandResult> results = new List<CommandResult>();
				foreach (var command in deployment.Commands)
				{
					var result = command.Execute(client, module);
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
