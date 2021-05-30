using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Health
{
    public class Program
    {
        public const string BasePath = @"..\TestData\output\fhir";

        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(o =>
            {
                o.AddDebug();
                o.AddConsole();
            });

            var provider = services.BuildServiceProvider();

            foreach (var blob in Directory.GetFiles(BasePath))
            {
                using var fileStream = File.OpenRead(blob);
                
                await FhirBundleBlobTrigger.Run(fileStream, blob, provider.GetRequiredService<ILogger<Program>>());
            }
        }
    }
}
