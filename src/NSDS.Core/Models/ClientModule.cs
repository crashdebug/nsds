using Newtonsoft.Json;

namespace NSDS.Core.Models
{
	public class ClientModule
    {
		[JsonProperty("module")]
		public Module Module { get; set; }
		[JsonProperty("version")]
		public BaseVersion Version { get; set; }
		[JsonProperty("isLatest")]
		public bool IsLatest { get => this.Version != null && this.Version.CompareTo(this.Module.Package.Version) <= 0; }
    }
}
