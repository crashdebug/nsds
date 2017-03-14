using System.Linq;
using Newtonsoft.Json.Linq;
using NSDS.Core.Interfaces;
using NSDS.Core.Models;

namespace NSDS.Core.Consumers
{
	public class VersionConsumer
	{
		private readonly IEventService eventService;

		public VersionConsumer(IEventService eventService)
		{
			this.eventService = eventService;
		}

		public void CheckVersion(object[] args)
		{
			if (args.Length != 3)
			{
				return;
			}

			var client = args[0] as Client;
			var module = args[1] as Module;
			var arr = args[2] as JArray;
			if (client == null || module == null || arr == null)
			{
				return;
			}

			for (int i = 0; i < arr.Count; i++)
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
			}
		}
	}
}
