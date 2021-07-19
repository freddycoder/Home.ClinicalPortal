using System;
using Blazored.Modal;
using BlazorOnFhir.Authentication;
using BlazorOnFhir.Extensions;
using BlazorOnFhir.Services;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.UI;

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
            services.AddForwardedHeaders(Configuration);
            
            var mvcBuilder = services.AddRazorPages();
            if (AddAuthenticationExtension.IsAzureADAuth(Configuration))
            {
                mvcBuilder.AddMvcOptions(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                  .RequireAuthenticatedUser()
                                  .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }).AddMicrosoftIdentityUI();
            }
            services.AddControllers();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddBlazorOnFhirAutorisation(Configuration);

            services.AddSingleton(new Hl7.Fhir.Serialization.FhirJsonParser());
            services.AddSingleton(new Hl7.Fhir.Serialization.FhirJsonSerializer(new Hl7.Fhir.Serialization.SerializerSettings
            {
                Pretty = true
            }));
            services.AddSingleton(new HttpClientConfig
            {
                Url = Environment.GetEnvironmentVariable("FHIR_API_URL"),
                BearerToken = Environment.GetEnvironmentVariable("FHIR_BEARER_TOKEN"),
                ClientId = Environment.GetEnvironmentVariable("CLIENT_ID"),
                TenantId = Environment.GetEnvironmentVariable("TENENT_ID"),
                Resource = Environment.GetEnvironmentVariable("FHIR_API_URL"),
                ClientSecret = Environment.GetEnvironmentVariable("FHIR_CLIENT_CREDENTIAL")
            });
            services.AddScoped(sp =>
            {
                var c = sp.GetRequiredService<HttpClientConfig>();

                if (c.UseClientCredentials)
                {
                    return new FHIRProxy.FHIRClient(c.Url, c.Resource, c.TenantId, c.ClientId, c.ClientSecret);
                }

                return new FHIRProxy.FHIRClient(c.Url, c.BearerToken);
            });
            services.AddSingleton(new HtmlSanitizer());
            services.AddSingleton<FhirJsonExampleService>();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UsePathBase")) == false)
            {
                app.UsePathBase(Environment.GetEnvironmentVariable("UsePathBase"));
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("StartsWithSegments")) == false)
            {
                app.Use((context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments(Environment.GetEnvironmentVariable("StartsWithSegments"), out var remainder))
                    {
                        context.Request.Path = remainder;
                    }

                    return next();
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseForwardedHeadersRules(logger, Configuration);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseForwardedHeadersRules(logger, Configuration);

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
