using System.Collections.Generic;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class ModuleDataModel : Module
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		public virtual ICollection<ClientModuleDataModel> Clients { get; set; }
	}
}
