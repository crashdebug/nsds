using System;
using System.ComponentModel.DataAnnotations;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public sealed class ModuleDataModel
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Endpoint { get; set; }

		public BaseVersion Version { get; set; }

		public string VersionId { get; set; }

		public int? DeploymentId { get; set; }

		public DeploymentDataModel Deployment { get; set; }

		internal Module ToModule()
		{
			return new Module
			{
				Name = this.Name,
				Endpoint = this.Endpoint,
				Deployment = this.Deployment?.ToDeployment(),
				Version = this.Version,
			};
		}
	}
}
