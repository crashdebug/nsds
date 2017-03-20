using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

		[JsonIgnore]
		public virtual ICollection<ClientModuleDataModel> ClientModules { get; set; }

		[NotMapped]
		[JsonProperty("modules")]
		public override IEnumerable<Module> Modules
		{
			get => this.ClientModules.Select(x => x.Module).AsEnumerable<Module>();
			set => throw new NotSupportedException();
		}
	}
}
