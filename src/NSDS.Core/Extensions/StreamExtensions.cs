using System.IO;

namespace NSDS.Core.Extensions
{
	public static class StreamExtensions
    {
		public static void Write(this Stream stream, byte[] buffer)
		{
			if (buffer != null)
			{
				stream.Write(buffer, 0, buffer.Length);
			}
		}
    }
}
