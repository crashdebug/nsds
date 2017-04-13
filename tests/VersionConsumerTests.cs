using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Jobs;
using NSDS.Core.Models;
using NSDS.Core.Services;

namespace NSDS.Tests
{
	[TestClass]
	public class VersionConsumerTests
	{
		[TestMethod]
		public void PackageVersionTest()
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

			eventService.Register(Constants.Events.PackageVersionReceived, args =>
			{
			});

			var deploymentService = new DeploymentService(eventService, factory, consumer);
			var result = deploymentService.Deploy(package).Result;
		}

		[TestMethod]
		public void CartVersionTest()
		{
			var factory = new ConnectionFactory();
			var conn = new TestConnection(@"[
				{
					""key"": ""commit_date"",
					""value"": ""2017-03-01 23:35:17""
				},
				{
					""key"": ""branch"",
					""value"": ""release/release-0.8""
				},
				{
					""key"": ""commit"",
					""value"": ""e2026d06853b6e05da0e38cd302ef1063d65cc5c""
				},
				{
					""key"": ""deployed"",
					""value"": ""2017-03-03 14:11:56""
				}
			]");
			factory.Add("http", uri => conn);

			var clientService = new InMemoryClientService(
				new ClientModel
				{
					Name = "test",
					Address = "127.0.0.1",
					Enabled = true,
					Modules = new List<Module>(new[] {
						new ModuleModel
						{
							Name = "ui",
							Endpoint = new VersionResource { Url = "http://{0}:8000/cart/git" }
						}
					})
				}
			);

			var eventService = new EventService();
			eventService.Register(Constants.Events.ModuleVersionReceived, args =>
			{
			});

			var poller = new VersionPoller(clientService, clientService, factory, eventService);
			poller.Run();
			Assert.IsTrue(poller.Status == JobStatus.Success);
		}

		private class TestVersionParser<T> : IVersionParser where T : BaseVersion
		{
			private readonly Regex pattern;
			public Regex Pattern => this.pattern;

			public TestVersionParser(string pattern)
			{
				this.pattern = new Regex(pattern, RegexOptions.Compiled);
			}

			public BaseVersion Parse(string input)
			{
				return Activator.CreateInstance(typeof(T), input) as BaseVersion;
			}
		}
	}

	public class TestConnection : IConnection
	{
		private readonly string content;

		public TestConnection(string content)
		{
			this.content = content;
		}

		public void Dispose()
		{
		}

		public Task<Stream> GetStream()
		{
			return Task.Run<Stream>(() => new MemoryStream(Encoding.UTF8.GetBytes(this.content)));
		}
	}
}
