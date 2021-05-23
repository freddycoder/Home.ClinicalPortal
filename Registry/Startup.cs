using FindCandidate.Data;
using FindCandidate.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHL7.Contract;
using SoapCore;
using System.ServiceModel;

namespace MyHL7
{
    public class Startup
    {
		public void ConfigureServices(IServiceCollection services)
		{
			services.TryAddSingleton<IPatientRegistry, PatientRegistryContract>();
			services.AddMvc(x => x.EnableEndpointRouting = false);
			services.AddSoapCore();
			services.AddDbContext<FindCandidateDbContext>(o =>
			{
				o.UseInMemoryDatabase("FindCandidateInMemory");
			});
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
