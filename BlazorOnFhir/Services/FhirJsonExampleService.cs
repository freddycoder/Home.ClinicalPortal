using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorOnFhir.Services
{
    public class FhirJsonExampleService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FhirJsonExampleService> _logger;

        public FhirJsonExampleService(ILogger<FhirJsonExampleService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string> GetExample(string type)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://hl7.org/fhir/R4/{type.ToLower()}-example.json");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return $"Example not found";
        }
    }
}
