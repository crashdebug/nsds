using System.Collections.Generic;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PoolDataModel : Pool
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("clients")]
		public virtual ICollection<ClientDataModel> Clients { get; set; }
	}
}
