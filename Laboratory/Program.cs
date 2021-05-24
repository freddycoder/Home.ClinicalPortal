using Laboratory;
using Laboratory.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

			var db = host.Services.GetRequiredService<LaboratoryDbContext>();

			db.Patients.Add(new ResultEntity { });

			db.SaveChanges();

			host.Run();
		}
    }
}
