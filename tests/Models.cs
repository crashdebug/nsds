using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	class PoolModel : Pool
	{
		public int Id { get; set; }

		public List<Client> Clients = new List<Client>();
	}

	class ClientModel : Client
	{
		public ICollection<Module> Modules { get; internal set; }
	}

	class ModuleModel : Module
	{
	}

	class DeploymentModel : Deployment
	{
	}
}
