using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class DeploymentDataModel //: Deployment
    {
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public DateTime Created { get; set; }

		public virtual ICollection<DeploymentCommandsDataModel> DeploymentCommands { get; set; }

		[NotMapped]
		public ICollection<Command> Commands
		{
			get => this.DeploymentCommands.Select(x => x.Command?.Deserialize()).ToList();
			set
			{
				this.DeploymentCommands.Clear();
				int order = 0;
				foreach (var val in value)
				{
					this.DeploymentCommands.Add(new DeploymentCommandsDataModel
					{
						Deployment = this,
						Command = CommandDataModel.FromCommand(val),
						Order = order++,
					});
				}
			}
		}

		internal Deployment ToDeployment()
		{
			return new Deployment
			{
				Name = this.Name,
				Created = this.Created,
				Commands = this.Commands,
			};
		}

		public DeploymentDataModel()
		{
			this.DeploymentCommands = new List<DeploymentCommandsDataModel>();
		}
	}
}
