using Renci.SshNet;

namespace WebApplication.Commands
{
	public class SshExec : Command
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
	}
}
