using Home.ClinicalPortal.Model.Laboratory;
using Laboratory.Contract;
using Laboratory.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SoapCore;
using System.ServiceModel;

namespace Laboratory
{
    public class Startup
    {
		public void ConfigureServices(IServiceCollection services)
		{
			services.TryAddSingleton<ILaboratory, LaboratoryContract>();
			services.AddMvc(x => x.EnableEndpointRouting = false);
			services.AddSoapCore();
			services.AddDbContext<LaboratoryDbContext>(o =>
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
				e.UseSoapEndpoint<ILaboratory>(o =>
				{
					o.Binding = new BasicHttpBinding();
					o.IndentXml = true;
					o.Path = "/Laboratory/Results.svc";
				});
			});
		}
	}
}
