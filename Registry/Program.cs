using FindCandidate.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace MyHL7
{
    public class Program
    {
        public static void Main()
        {
			var host = new WebHostBuilder()
				.UseKestrel(x => x.AllowSynchronousIO = true)
				.UseUrls("http://*:5050")
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.ConfigureLogging(x =>
				{
					x.AddDebug();
					x.AddConsole();
				})
				.Build();

			var db = host.Services.GetRequiredService<FindCandidateDbContext>();

			db.Patients.Add(new PatientEntity
			{
				FirstName = "Fr�d�ric",
				LastName = "Jacques"
			});

			db.SaveChanges();

			host.Run();
		}
    }
}
