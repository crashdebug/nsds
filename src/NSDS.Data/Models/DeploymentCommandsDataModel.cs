using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using NSDS.Core;

namespace NSDS.Data.Models
{
    public class DeploymentCommandsDataModel
    {
		public int DeploymentId { get; set; }
		public DeploymentDataModel Deployment { get; set; }

		public string CommandName { get; set; }
		[ForeignKey("CommandName")]
		public Command Command { get; set; }

		public int Order { get; set; }
    }
}
