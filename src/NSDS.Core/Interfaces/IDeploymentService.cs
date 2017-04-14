using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IDeploymentService
    {
		Task<DeploymentResult> Deploy(Package package, IDictionary<string, object> environment = null, ILogger logger = null);
		Task<DeploymentResult> Deploy(Client client, ClientModule module, IDictionary<string, object> environment = null, ILogger logger = null);
	}

	public class DeploymentArguments
	{
		public Client Client { get; set; }
		public Module Module { get; set; }
		public Package Package { get; set; }
		public IDictionary<string, object> Environment { get; set; }
	}

	public class DeploymentResult
	{
		[JsonProperty("results")]
		public ICollection<CommandResult> CommandResults { get; private set; }
		[JsonProperty("success")]
		public bool Success { get => this.CommandResults.All(x => x.Success); }
		[JsonProperty("version")]
		public BaseVersion Version { get; internal set; }

		public DeploymentResult()
		{
			this.CommandResults = new List<CommandResult>();
		}

		internal void Add(CommandResult result)
		{
			this.CommandResults.Add(result);
		}
	}
}
