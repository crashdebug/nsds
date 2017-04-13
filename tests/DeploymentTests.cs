using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
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
				Endpoint = new VersionResource { Url = "http://{0}/test" },
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
			var eventService = new EventService();
			var versionConsumer = new VersionResolver { new DateVersion() };
			IDeploymentService service = new DeploymentService(eventService, new ConnectionFactory(), versionConsumer);

			var result = await service.Deploy(client, module);

			Assert.AreEqual(1, result.CommandResults.Count());
			Assert.IsTrue(command.Executed);
			Assert.IsTrue(result.Success);
		}
	}

	internal class TestCommand : Command
	{
		public bool Executed { get; internal set; }

		public override Task Execute(DeploymentArguments args, CommandResult result, ILogger logger = null)
		{
			this.Executed = true;
			return Task.Run(() =>
			{
				result.Success = true;
			});
		}
	}
}
