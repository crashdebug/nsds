using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IDeploymentService
    {
		Task<IEnumerable<CommandResult>> DeployModule(Client client, Module module, Deployment deployment, ILogger logger = null);
	}
}
