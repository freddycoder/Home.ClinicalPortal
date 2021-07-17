using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Laboratory
{
    public class Program
    {
        public static void Main()
        {
			var host = new WebHostBuilder()
				.UseKestrel(x => x.AllowSynchronousIO = true)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.ConfigureLogging(x =>
				{
					x.AddDebug();
					x.AddConsole();
				})
				.Build();

			host.Run();
		}
    }
}
