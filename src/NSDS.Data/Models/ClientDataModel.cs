﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class ClientDataModel //: Client
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

		[NotMapped]
		public IEnumerable<Module> Modules
		{
			get => this.ClientModules.Select(x => x.Module.ToModule()).AsEnumerable();
		}
	}
}
