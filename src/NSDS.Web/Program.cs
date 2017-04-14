using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace NSDS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
			int port = 5000;
			if (args.Length > 0)
			{
				int.TryParse(args[0], out port);
			}

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
				.UseUrls($"http://localhost:{port}")
                .Build();

            host.Run();
        }
    }
}
