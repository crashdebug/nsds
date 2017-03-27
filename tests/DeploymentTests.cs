using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;
using NSDS.Core.Services;

namespace NSDS.Tests
{
	[TestClass]
	public class DeploymentTests
    {
		[TestMethod]
		public async void ModuleDeploymentTestAsync()
		{
			Module module = new ModuleModel
			{
				Name = "test_module",
				Endpoint = "http://{0}/test",
				Version = new NumericVersion(0, 7),
			};
			Client client = new ClientModel
			{
				Address = "127.0.0.1:8000",
				Enabled = true,
				Name = "test_client",
				Modules = new[] { module }
			};
			var command = new TestCommand();
			Deployment deployment = new DeploymentModel
			{
				Name = "test_deployment",
				Commands = new Command[] { command }
			};
			IDeploymentService service = new DeploymentService();

			var result = await service.DeployModule(client, module, deployment);

			Assert.AreEqual(1, result.Count());
			Assert.IsTrue(command.Executed);
			Assert.IsTrue(result.All(x => x.Success));
		}
	}

	internal class TestCommand : Command
	{
		public bool Executed { get; internal set; }

		public override CommandResult Execute(Client client, Module module, ILogger log)
		{
			this.Executed = true;
			return new CommandResult
			{
				Command = this,
				Success = true
			};
		}
	}
}
