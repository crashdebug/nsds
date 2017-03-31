using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PackageDataModel
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime Created { get; set; }

		public string Url { get; set; }

		public string VersionId { get; set; }
		[ForeignKey("VersionId")]
		public BaseVersion Version { get; set; }

		public int ModuleId { get; set; }
		[ForeignKey("ModuleId")]
		public ModuleDataModel Module { get; set; }

		public int? DeploymentId { get; set; }
		[ForeignKey("DeploymentId")]
		public DeploymentDataModel Deployment { get; set; }

		internal Package ToPackage()
		{
			return new Package
			{
				Created = this.Created,
				Deployment = this.Deployment?.ToDeployment(),
				Module = this.Module?.ToModule(),
				Name = this.Name,
				Url = this.Url,
				Version = this.Version,
			};
		}
	}
}
