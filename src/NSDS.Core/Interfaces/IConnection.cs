using System;
using System.IO;
using System.Threading.Tasks;

namespace NSDS.Core.Interfaces
{
	public interface IConnection : IDisposable
	{
		Task<Stream> GetStream();
	}
}
