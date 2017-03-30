using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NSDS.Core;
using NSDS.Core.Models;

namespace NSDS.Data.Models
{
	public class DeploymentDataModel //: Deployment
    {
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonIgnore]
		public virtual ICollection<DeploymentCommandsDataModel> DeploymentCommands { get; set; }

		[NotMapped]
		public ICollection<Command> Commands
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
						Command = new CommandDataModel
						{
							Discriminator = val.GetType().FullName,
							Name = val.Name,
							Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val))
						},
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
