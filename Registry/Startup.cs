using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Registry.Contract;
using SoapCore;
using System.ServiceModel;
using Home.ClinicalPortal.Model.Registry;
using FHIRProxy;
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Registry
{
    public class Startup
    {
		public void ConfigureServices(IServiceCollection services)
		{
			services.TryAddSingleton<IPatientRegistry, PatientRegistryContract>();
			services.AddMvc(x => x.EnableEndpointRouting = false);
			services.AddSoapCore();
			services.AddScoped(sp => new FHIRClient(baseurl: Environment.GetEnvironmentVariable("FHIR_API_URL"),
													bearerToken: Environment.GetEnvironmentVariable("FHIR_BEARER_TOKEN")));
			//services.AddScoped(sp => new FHIRClient(baseurl: Environment.GetEnvironmentVariable("FHIR_API_URL"),
			//									      tenent: Environment.GetEnvironmentVariable("TENENT_ID"),
			//										  clientid: Environment.GetEnvironmentVariable("CLIENT_ID"),
			//										  resource: Environment.GetEnvironmentVariable("FHIR_API_URL"),
			//										  secret: Environment.GetEnvironmentVariable("FHIR_CLIENT_CREDENTIAL")));
			services.AddSingleton(new Hl7.Fhir.Serialization.FhirJsonParser());
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseDeveloperExceptionPage();

			if (string.Equals(Environment.GetEnvironmentVariable("DEBUG_REQUEST_BODY"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
				app.Use(async (context, next) =>
				{
					context.Request.EnableBuffering();

					using (var reader = new StreamReader(
									context.Request.Body,
									encoding: Encoding.UTF8,
									detectEncodingFromByteOrderMarks: false,
									bufferSize: 30 * 1024,
									leaveOpen: true))
					{
						var body = await reader.ReadToEndAsync();
						// Do some processing with body…

						// Reset the request body stream position so the next middleware can read it
						context.Request.Body.Position = 0;
					}

					await next();
				});
			}

			app.UseRouting();

			app.UseEndpoints(e =>
			{
				e.UseSoapEndpoint<IPatientRegistry>(o =>
				{
					o.Binding = new BasicHttpBinding();
					o.IndentXml = true;
					o.Path = "/Registry/PatientRegistry.svc";
				});

				e.UseSoapEndpoint<IPersonRegistry>(o =>
				{
					o.Binding = new BasicHttpBinding();
					o.IndentXml = true;
					o.Path = "/Registry/PersonRegistry.svc";
				});
			});
		}
	}
}
