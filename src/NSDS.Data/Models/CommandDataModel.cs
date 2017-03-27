using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using NSDS.Core;

namespace NSDS.Data.Models
{
	public class CommandDataModel
    {
		[Required]
		public string Name { get; set; }
		[Required]
		public string Discriminator { get; set; }
		[Required]
		public byte[] Payload { get; set; }

		public CommandDataModel()
		{
		}

		public CommandDataModel(Command val)
		{
			this.Name = val.Name;
			this.Discriminator = val.GetType().FullName;
			this.Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val));
		}

		internal Command Deserialize()
		{
			Type t = Type.GetType(this.Discriminator);
			return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(this.Payload), t) as Command;
		}
	}
}
