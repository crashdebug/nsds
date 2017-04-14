using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PoolDataModel
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public DateTime Created { get; set; }

		public virtual ICollection<ClientDataModel> Clients { get; set; }

		internal Pool ToPool(MappingContext context)
		{
			return context.Get(this, x => x.Name, () => new Pool
				{
					Name = this.Name,
					Created = this.Created,
					Clients = this.Clients?.Select(x => x.ToClient(context)).AsEnumerable(),
				})();
		}
	}
}
