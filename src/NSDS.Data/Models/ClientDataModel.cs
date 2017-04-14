using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class ClientDataModel
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Address { get; set; }

		public bool Enabled { get; set; }

		public DateTime Created { get; set; }

		public int? PoolId { get; set; }
		[ForeignKey("PoolId")]
		public virtual PoolDataModel Pool { get; set; }

		public virtual ICollection<ClientModuleDataModel> ClientModules { get; set; }

		internal Client ToClient(MappingContext context)
		{
			return context.Get(this, x => x.Name, () => new Client
				{
					Address = this.Address,
					Created = this.Created,
					Enabled = this.Enabled,
					Modules = this.ClientModules.Select(x => new ClientModule { Module = x.Module.ToModule(context), Version = x.Version }).ToList(),
					Name = this.Name,
				})();
		}
	}
}
