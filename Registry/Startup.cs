using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Registry.Contract;
using SoapCore;
using System.ServiceModel;
using Home.ClinicalPortal.Model.Registry;
using FHIRProxy;
using System;

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
			//									    tenent: Environment.GetEnvironmentVariable("TENENT_ID"),
			//										clientid: Environment.GetEnvironmentVariable("CLIENT_ID"),
			//										resource: Environment.GetEnvironmentVariable("FHIR_API_URL"),
			//										secret: Environment.GetEnvironmentVariable("FHIR_CLIENT_CREDENTIAL")));
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseDeveloperExceptionPage();

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
