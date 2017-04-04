using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSDS.Core.Interfaces;

namespace NSDS.Core.Commands
{
	public class ShellCommand : Command
	{
		public string Command { get; set; }
		public string Arguments { get; set; }
		public string WorkingDirectory { get; set; }
		public TimeSpan? Timeout { get; set; }
		public int[] SuccessCodes { get; set; }

		public ShellCommand(string command, string arguments, string workingDir = null, TimeSpan? timeout = null)
		{
			this.Command = command;
			this.Arguments = arguments;
			this.WorkingDirectory = workingDir;
			this.Timeout = timeout;
			this.SuccessCodes = new int[] { 0 };
		}

		public override Task<CommandResult> Execute(DeploymentArguments args, CommandResult result, ILogger logger = null)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					Arguments = this.Arguments,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					// Must be false
					UseShellExecute = false,
					WorkingDirectory = args.Environment.ContainsKey("workingDir") ? args.Environment["workingDir"].ToString() : this.WorkingDirectory,
					FileName = this.Command,
				},
			};

			process.OutputDataReceived += (sender, arg) => logger?.LogInformation(arg.Data);
			process.ErrorDataReceived += (sender, arg) => logger?.LogError(arg.Data);

			return Task.Run(() =>
			{
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				if (!this.Timeout.HasValue)
				{
					process.WaitForExit();
				}
				if (!this.Timeout.HasValue || process.WaitForExit(Convert.ToInt32(this.Timeout.Value.TotalMilliseconds)))
				{
					result.Success = this.SuccessCodes.Contains(process.ExitCode);
				}
				process.Dispose();
				return result;
			});
		}
	}
}
