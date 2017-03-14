using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSDS.Core;
using NSDS.Core.Consumers;
using NSDS.Core.Interfaces;
using NSDS.Core.Jobs;
using NSDS.Core.Models;

namespace NSDS.Tests
{
	[TestClass]
	public class VersionConsumerTests
	{
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
							Endpoint = "http://{0}:8000/cart/git"
						}
					})
				}
			);

			var eventService = new EventService();
			eventService.Register("VersionReceived", new VersionConsumer(eventService).CheckVersion);

			var poller = new VersionPoller(clientService, clientService, factory, eventService);
			poller.Run();
			Assert.IsTrue(poller.Status == JobStatus.Success);
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
