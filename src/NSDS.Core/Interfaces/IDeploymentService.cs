using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IDeploymentService
    {
		Task<IEnumerable<CommandResult>> DeployModule(Deployment deployment, DeploymentArguments args, ILogger logger = null);
	}

	public class DeploymentArguments
	{
		public Client Client { get; set; }
		public Module Module { get; set; }
		public IDictionary<string, object> Environment { get; set; }
	}
}
