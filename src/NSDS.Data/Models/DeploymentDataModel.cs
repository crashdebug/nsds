using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class DeploymentDataModel : Deployment
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonIgnore]
		public virtual ICollection<DeploymentCommandsDataModel> DeploymentCommands { get; set; }

		[NotMapped]
		public override ICollection<Command> Commands
		{
			get => this.DeploymentCommands.Select(x => x.Command.Deserialize()).ToList();
			set
			{
				this.DeploymentCommands.Clear();
				int order = 0;
				foreach (var val in value)
				{
					this.DeploymentCommands.Add(new DeploymentCommandsDataModel
					{
						Deployment = this,
						Command = new CommandDataModel(val),
						Order = order++,
					});
				}
			}
		}
	}
}
