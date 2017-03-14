using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class ClientDataModel : Client
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		[ForeignKey("PoolId")]
		public virtual PoolDataModel Pool { get; set; }

		[JsonProperty("poolId")]
		public int PoolId { get; set; }

		[JsonProperty("modules")]
		public virtual ICollection<ClientModuleDataModel> Modules { get; set; }
	}
}
