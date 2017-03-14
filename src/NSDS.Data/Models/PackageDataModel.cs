using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PackageDataModel : Package
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		public string VersionId { get; set; }
	}
}
