using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorOnFhir
{
    public class HttpClientConfig
    {
        public string Url { get; set; }

        public string BearerToken { get; set; }
    }
}
