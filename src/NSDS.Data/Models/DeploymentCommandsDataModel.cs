using System.ComponentModel.DataAnnotations.Schema;

namespace NSDS.Data.Models
{
	public class DeploymentCommandsDataModel
    {
		public int DeploymentId { get; set; }
		public DeploymentDataModel Deployment { get; set; }

		public string CommandName { get; set; }
		[ForeignKey("CommandName")]
		public CommandDataModel Command { get; set; }

		public int Order { get; set; }
    }
}
