using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorOnFhir.Authentication
{
    /// <summary>
    /// When no authentification method is configured. This authorization handler is used.
    /// </summary>
    public class AllowAnonymousStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        private static readonly AuthenticationState _user = InstanciateUser();

        /// <summary>
        /// This implementation return always the same user. This class should only be used in devloppement.
        /// </summary>
        /// <returns></returns>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(_user);
        }

        /// <summary>
        /// This function do nothing. This should only be used in devloppment
        /// </summary>
        /// <param name="authenticationStateTask"></param>
        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {

        }

        /// <summary>
        /// Create the default authentication state for the AllowAnonymousStateProvider
        /// </summary>
        /// <returns></returns>
        private static AuthenticationState InstanciateUser()
        {
            List<Claim> claims = new() { new Claim(ClaimTypes.Name, "User") };

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal user = new(claimsIdentity);

            return new AuthenticationState(user);
        }
    }
}
