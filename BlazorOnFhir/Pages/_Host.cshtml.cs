using BlazorOnFhir.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorOnFhir.Pages
{
    public class _HostModel : PageModel
    {
        public _HostModel(UrlService urlService)
        {
            UrlService = urlService;
        }

        public UrlService UrlService { get; }

        public void OnGet()
        {
            
        }
    }
}
