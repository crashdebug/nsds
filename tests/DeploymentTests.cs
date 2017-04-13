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
		public void PackageDeploymentTest()
		{
			var factory = new ConnectionFactory();
			var conn = new TestConnection(@"{
			  ""version"": ""0.1.0"",
			  ""created"": ""2017-04-07T23:09:30.5972588Z"",
			  ""name"": ""NSDS.Web.0.1.0.nupkg""
			}");
			factory.Add("http", uri => conn);

			var package = new Package
			{
				Deployment = new Deployment
				{
					Commands = new Command[0],
					Name = "test"
				},
				Module = new Module
				{
					Name = "test",
				},
				Endpoint = new VersionResource
				{
					Url = "http://localhost",
					PathQuery = "/root/version"
				}
			};

			var eventService = new EventService();
			var consumer = new VersionResolver { new NumericVersion() };
			bool eventReceived = false;
			eventService.Register(Constants.Events.PackageVersionReceived, args =>
			{
				eventReceived = true;
			});

			var deploymentService = new DeploymentService(eventService, factory, consumer);
			var result = deploymentService.Deploy(package).Result;

			Assert.IsTrue(result.Success);
			Assert.IsTrue(eventReceived);
			Assert.IsNotNull(result.Version);
		}

		[TestMethod]
		public void ModuleDeploymentTest()
		{
			var factory = new ConnectionFactory();
			factory.Add("http", uri => new TestConnection(@"{
			  'version': '0.8.0',
			  'created': '2017-04-07T23:09:30.5972588Z'
			}"));

			var command = new TestCommand();
			Deployment deployment = new DeploymentModel
			{
				Name = "test_deployment",
				Commands = new Command[] { command },
			};
			Module module = new ModuleModel
			{
				Name = "test_module",
				Endpoint = new VersionResource { Url = "http://{0}/test", PathQuery = "/root/version" },
				Version = new NumericVersion(0, 7),
				Deployment = deployment,
			};
			Client client = new ClientModel
			{
				Address = "127.0.0.1:8000",
				Enabled = true,
				Name = "test_client",
				Modules = new[] { module },
			};
			var eventService = new EventService();
			var versionConsumer = new VersionResolver { new NumericVersion() };
			var service = new DeploymentService(eventService, factory, versionConsumer);

			var result = service.Deploy(client, module).Result;

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
