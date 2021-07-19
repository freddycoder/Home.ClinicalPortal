using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorOnFhir.Services
{
    public class UrlService
    {
        private readonly string? _basePath;

        public UrlService(string? basePath = null)
        {
            _basePath = basePath;
        }

        public string Url(string url)
        {
            return $"{_basePath}{url}";
        }
    }
}
