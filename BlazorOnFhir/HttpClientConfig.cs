namespace BlazorOnFhir
{
    public class HttpClientConfig
    {
        public string Url { get; set; }

        public string BearerToken { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string Resource { get; set; }

        public string ClientSecret { get; set; }

        public bool UseClientCredentials => !string.IsNullOrEmpty(ClientId);
    }
}
