using System.ComponentModel.DataAnnotations.Schema;
using NSDS.Core;

namespace NSDS.Data.Models
{
	public class ClientModuleDataModel
    {
		public int ClientId { get; set; }
		[ForeignKey("ClientId")]
		public ClientDataModel Client { get; set; }

		public int ModuleId { get; set; }
		[ForeignKey("ModuleId")]
		public ModuleDataModel Module { get; set; }

		public string VersionId { get; set; }
		[ForeignKey("VersionId")]
		public BaseVersion Version { get; set; }
    }
}
