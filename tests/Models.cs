using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	class PoolModel : Pool
	{
		public int Id { get; set; }

		public new List<Client> Clients = new List<Client>();
	}

	class ClientModel : Client
	{
		public int Id { get; internal set; }
	}

	class ModuleModel : Module
	{
		public int Id { get; internal set; }
	}

	class DeploymentModel : Deployment
	{
		public int Id { get; internal set; }
	}
}
