using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;

namespace BlazorOnFhir.Authentication
{
    public static class AddAuthenticationExtension
    {
        /// <exception cref="InvalidProgramException" />
        public static IServiceCollection AddBlazorOnFhirAutorisation(this IServiceCollection services, IConfiguration configuration)
        {
            if (!OnlyOneAuthMethodIsUsed(configuration))
            {
                throw new InvalidProgramException("You cannot use cookie user and identity at the same time. See the github wiki for more information.");
            }

            if (IsAzureADAuth(configuration))
            {
                services.AddSingleton(new AuthUrlPagesProvider("AzureAD"));

                // Support for kubernetes
                // Colon is not allow in environement variable name
                if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:ClientId")) &&
                    !string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__ClientId")))
                {
                    configuration["AzureAD:ClientId"] = configuration.GetValue<string>("AzureAD__ClientId");
                }

                if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:TenantId")) &&
                    !string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__TenantId")))
                {
                    configuration["AzureAD:TenantId"] = configuration.GetValue<string>("AzureAD__TenantId");
                }

                if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:CallbackPath")) &&
                    !string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__CallbackPath")))
                {
                    configuration["AzureAD:CallbackPath"] = configuration.GetValue<string>("AzureAD__CallbackPath");
                }

                if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:SignedOutCallbackPath")) && !string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__SignedOutCallbackPath")))
                {
                    configuration["AzureAD:SignedOutCallbackPath"] = configuration.GetValue<string>("AzureAD__SignedOutCallbackPath");
                }

                if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:Instance")) &&
                    !string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__Instance")))
                {
                    configuration["AzureAD:Instance"] = configuration.GetValue<string>("AzureAD__Instance");
                }

                services.AddMicrosoftIdentityWebAppAuthentication(configuration);
            }
            else
            {
                services.AddSingleton(new AuthUrlPagesProvider(""));

                services.AddSingleton<IHostEnvironmentAuthenticationStateProvider, AllowAnonymousStateProvider>();
                services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();
            }

            return services;
        }

        /// <summary>
        /// Method use to validate the configuration of the app. Only one method should be identify.
        /// </summary>
        /// <returns></returns>
        public static bool OnlyOneAuthMethodIsUsed(IConfiguration configuration)
        {
            int count = 0;

            if (IsAzureADAuth(configuration)) count++;

            return count <= 1;
        }

        /// <summary>
        /// Indicate if the AzureAD authentication mecanism is configure
        /// </summary>
        /// <returns></returns>
        public static bool IsAzureADAuth(IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD:TenantId")) &&
                string.IsNullOrWhiteSpace(configuration.GetValue<string>("AzureAD__TenantId")))
            {
                return false;
            }

            return true;
        }
    }
}
