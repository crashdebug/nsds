using System.ComponentModel.DataAnnotations.Schema;

namespace NSDS.Data.Models
{
	public class DeploymentCommandsDataModel
    {
		public DeploymentCommandsDataModel()
		{
		}

		public DeploymentCommandsDataModel(DeploymentCommandsDataModel dc, CommandDataModel c)
		{
			this.Deployment = dc.Deployment;
			this.DeploymentId = dc.DeploymentId;
			this.Order = dc.Order;
			this.CommandName = dc.CommandName;
			this.Command = c;
		}

		public int DeploymentId { get; set; }
		[ForeignKey("DeploymentId")]
		public DeploymentDataModel Deployment { get; set; }

		public string CommandName { get; set; }
		[ForeignKey("CommandName")]
		public CommandDataModel Command { get; set; }

		public int Order { get; set; }
    }
}
