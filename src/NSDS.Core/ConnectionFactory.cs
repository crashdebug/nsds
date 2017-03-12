using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NSDS.Core.Interfaces;

namespace NSDS.Core
{
	public class ConnectionFactory : IDisposable
	{
		private IDictionary<string, Func<Uri, IConnection>> table = new Dictionary<string, Func<Uri, IConnection>>();

		public ConnectionFactory Add(string protocol, Func<Uri, IConnection> conn)
		{
			this.table[protocol] = conn;
			return this;
		}

		public IConnection CreateConnection(Uri uri)
		{
			if (!this.table.ContainsKey(uri.Scheme))
			{
				return null;
			}
			return this.table[uri.Scheme](uri);
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
					this.table.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConnectionFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class HttpConnection : IConnection
    {
		private readonly Uri address;

		public HttpConnection(Uri address)
		{
			this.address = address;
		}

		public async Task<Stream> GetStream()
		{
			using (HttpClient http = new HttpClient())
			{
				http.BaseAddress = this.address;
				return await http.GetStreamAsync(this.address);
			}
		}

		public void Dispose()
        {
        }
    }
}
