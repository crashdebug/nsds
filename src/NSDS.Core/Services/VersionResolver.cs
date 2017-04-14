using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NSDS.Core.Services
{
	public class VersionResolver : IEnumerable<IVersionParser>
	{
		private readonly ConnectionFactory connectionFactory;
		private readonly ILogger logger;
		private List<IVersionParser> parsers = new List<IVersionParser>();

		public VersionResolver(ConnectionFactory connectionFactory, ILogger logger = null)
		{
			this.connectionFactory = connectionFactory;
			this.logger = logger;
		}

		public void Add(IVersionParser parser)
		{
			this.parsers.Add(parser);
		}

		public async Task<BaseVersion> GetVersion(VersionResource resource)
		{
			if (string.IsNullOrWhiteSpace(resource.Url))
			{
				return null;
			}
			using (var conn = this.connectionFactory.CreateConnection(new Uri(resource.Url)))
			{
				using (var reader = new StreamReader(await conn.GetStream()))
				{
					return this.CheckVersion(JsonConvert.DeserializeXmlNode(await reader.ReadToEndAsync(), "root"), resource.PathQuery);
				}
			}
		}

		private BaseVersion CheckVersion(XmlDocument doc, string q)
		{
			if (string.IsNullOrWhiteSpace(q))
			{
				return null;
			}
			var arr = doc.SelectNodes(q);
			if (arr.Count == 0)
			{
				this.logger?.LogWarning("VersionConsumer.CheckVersion(): Query '{0}' did not yield any results. Document:\n{1}", q, doc.OuterXml);
				return null;
			}

			var versionString = arr.Item(0).InnerText;
			this.logger?.LogDebug("VersionConsumer.CheckVersion(): Version '{0}' found", versionString);

			foreach (var parser in this.parsers)
			{
				if (!parser.Pattern.IsMatch(versionString))
				{
					continue;
				}
				var version = parser.Parse(versionString);
				this.logger?.LogDebug("VersionConsumer.CheckVersion(): Found matching parser for version '{0}' with pattern '{1}'; Result: {2}", versionString, parser.Pattern, version);
				return version;
			}
			this.logger?.LogWarning("VersionConsumer.CheckVersion(): Could not find a matching version for '{0}'", versionString);
			return null;

			/*for (int i = 0; i < arr.Count; i++)
			{
				var kvp = arr.ElementAt(i).ToObject<JObject>();
				if (!kvp.TryGetValue("key", out JToken val))
				{
					return;
				}
				switch (val.ToString())
				{
					case "commit_date":
						if (kvp.TryGetValue("value", out val))
						{
							BaseVersion version = new DateVersion(val.ToString());
							if (module.Version.CompareTo(version) != 0)
							{
								var oldVersion = module.Version;
								module.Version = version;
								this.eventService.Invoke("VersionChanged", client, module, oldVersion, module.Version);
							}
						}
						break;
				}
			}*/
		}

		public IEnumerator<IVersionParser> GetEnumerator()
		{
			return this.parsers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
