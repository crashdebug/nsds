using System;
using System.Collections.Generic;
using NSDS.Core;
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
		public int Id { get; internal set; }
		public override IEnumerable<Module> Modules { get; set; }
	}

	class ModuleModel : Module
	{
		public int Id { get; internal set; }
	}

	class DeploymentModel : Deployment
	{
		public override ICollection<Command> Commands { get; set; }
	}
}
