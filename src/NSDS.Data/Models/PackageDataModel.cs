using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class PackageDataModel //: Package
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime Created { get; set; }

		public BaseVersion Version { get; set; }

		public string Url { get; set; }

		public string VersionId { get; set; }

		public int ModuleId { get; set; }
		[ForeignKey("ModuleId")]
		public ModuleDataModel Module { get; set; }

		public int? DeploymentId { get; set; }
		[ForeignKey("DeploymentId")]
		public DeploymentDataModel Deployment { get; set; }
	}
}
