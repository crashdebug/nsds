using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

		public int? PackageId { get; set; }
		[ForeignKey("PackageId")]
		public PackageDataModel Package { get; set; }

		public int? DeploymentId { get; set; }
		[ForeignKey("DeploymentId")]
		public DeploymentDataModel Deployment { get; set; }

		[Required]
		public string PathQuery { get; set; }

		internal Module ToModule(MappingContext context)
		{
			return context.Get(this, x => x.Name, () => new Module
				{
					Name = this.Name,
					Endpoint = new VersionResource
					{
						Url = this.Endpoint,
						PathQuery = this.PathQuery
					},
					Deployment = this.Deployment?.ToDeployment(context),
					Package = this.Package?.ToPackage(context),
				})();
		}
	}
}
