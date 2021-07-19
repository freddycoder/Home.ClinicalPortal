using BlazorOnFhir.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlazorOnFhir.Controllers
{
    /// <summary>
    /// Controller used to manage redirection with Microsoft to login and logout users
    /// </summary>
    [Route("[controller]/[action]")]
    public class AzureADController : ControllerBase
    {
        private readonly UrlService _urlService;

        public AzureADController(UrlService urlService)
        {
            _urlService = urlService;
        }

        /// <summary>
        /// Login method to handle Microsoft authentication
        /// </summary>
        /// <param name="returnUrl">The returnUrl use after the microsoft authentication is completed</param>
        [HttpGet]
        public async Task<ActionResult> Login(string returnUrl)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
            };

            return await Task.Run(() => Challenge(props));
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect(_urlService.Url("/"));
        }
    }
}
