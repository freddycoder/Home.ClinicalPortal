using System;
using Blazored.Modal;
using BlazorOnFhir.Services;
using Ganss.XSS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorOnFhir
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();
            services.AddSingleton(new Hl7.Fhir.Serialization.FhirJsonParser());
            services.AddSingleton(new Hl7.Fhir.Serialization.FhirJsonSerializer(new Hl7.Fhir.Serialization.SerializerSettings
            {
                Pretty = true
            }));
            services.AddSingleton(new HttpClientConfig
            {
                Url = Environment.GetEnvironmentVariable("FHIR_API_URL")
            });
            services.AddScoped(sp =>
            {
                var c = sp.GetRequiredService<HttpClientConfig>();

                return new FHIRProxy.FHIRClient(c.Url, c.BearerToken);
            });
            services.AddSingleton(new HtmlSanitizer());
            services.AddSingleton<FhirJsonExampleService>();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
