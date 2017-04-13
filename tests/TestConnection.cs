using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSDS.Core.Interfaces;

namespace NSDS.Tests
{
	public class TestConnection : IConnection
	{
		private readonly string content;

		public TestConnection(string content)
		{
			this.content = content;
		}

		public void Dispose()
		{
		}

		public Task<Stream> GetStream()
		{
			return Task.Run<Stream>(() => new MemoryStream(Encoding.UTF8.GetBytes(this.content)));
		}
	}
}
