using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorOnFhir
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) 
            {
                var ser = host.Services.GetRequiredService<Hl7.Fhir.Serialization.FhirJsonParser>();

                var cache = host.Services.GetRequiredService<IMemoryCache>();

                // Saving the capabilities because it is a little slow to always query the metadata endpoint
                const string filesave = "capabilities.json";

                if (File.Exists(filesave))
                {
                    var capabilities = ser.Parse<CapabilityStatement>(await File.ReadAllTextAsync(filesave));

                    cache.Set(nameof(CapabilityStatement), capabilities);
                }
                else
                {
                    var fhirClient = scope.ServiceProvider.GetRequiredService<FHIRProxy.FHIRClient>();

                    var response = await fhirClient.LoadResource("metadata");

                    await File.WriteAllTextAsync(filesave, response.Content.ToString());

                    var capabilities = ser.Parse<CapabilityStatement>(response.Content.ToString());

                    cache.Set(nameof(CapabilityStatement), capabilities);
                }
                
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
