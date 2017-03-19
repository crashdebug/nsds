using System;
using NSDS.Core;
using NSDS.Core.Models;
using Renci.SshNet;

namespace NSDS.Web.Commands
{
	public class SshCommand : Command
	{
		private SshClient cli = null;

		public string Execute(string command)
		{
			var cmd = this.cli.CreateCommand(command);
			return cmd.Execute();
		}

		public void Connect(string address, int port, string username, string password)
		{
			this.cli = new SshClient(address, port, username, password);
			this.cli.Connect();
		}

		public void Disconnect()
		{
			if (this.cli != null)
			{
				this.cli.Disconnect();
				this.cli = null;
			}
		}

		public override CommandResult Execute(Client client, Module module)
		{
			throw new NotImplementedException();
		}
	}
}
