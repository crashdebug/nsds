using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NSDS.Core;

namespace NSDS.Data.Models
{
	public class ClientModuleDataModel
    {
		[JsonIgnore]
		public int ClientId { get; set; }
		[JsonIgnore]
		[ForeignKey("ClientId")]
		public ClientDataModel Client { get; set; }

		public int ModuleId { get; set; }
		[ForeignKey("ModuleId")]
		public ModuleDataModel Module { get; set; }

		[ForeignKey("VersionId")]
		[JsonIgnore]
		public BaseVersion Version { get; set; }
		public string VersionId { get; set; }
    }
}
