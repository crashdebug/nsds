using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
					Modules = new List<ClientModule>(new[] { 
						new ClientModule {
							Module = new ModuleModel
							{
								Name = "ui",
								Endpoint = new VersionResource { Url = "http://{0}:8000/cart/git" }
							}
						}
					})
				}
			);

			var eventService = new EventService();
			eventService.Register(Constants.Events.ModuleVersionReceived, args =>
			{
			});

			var versionResolver = new VersionResolver(factory) { new DateVersion() };

			var poller = new ModulePoller(clientService, factory, versionResolver, eventService);
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
}
