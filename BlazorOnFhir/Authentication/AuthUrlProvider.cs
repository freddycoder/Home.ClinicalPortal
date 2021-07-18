namespace BlazorOnFhir.Authentication
{
    /// <summary>
    /// Provider allow to get information about the authentication method used in the app
    /// and get basic url to handle authentication scenario
    /// </summary>
    public class AuthUrlPagesProvider
    {
        /// <summary>
        /// Constructor with the authentication shema used
        /// </summary>
        /// <param name="schema">Cookies, Identity.Application, AzureAD, or string.Empty</param>
        public AuthUrlPagesProvider(string schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// The schema that is used be the application
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// Return the url of the login page
        /// </summary>
        public string LoginPage => Schema switch
        {
            "AzureAD" => "/Logout",
            _ => "",
        };

        /// <summary>
        /// Return the url to the POST method to log out
        /// </summary>
        public string LogoutPage => Schema switch
        {
            "AzureAD" => "/identity/account/logout",
            _ => "",
        };

        /// <summary>
        /// Indicate if the authentication is enabled in the app
        /// </summary>
        public bool AuthenticationEnabled => Schema != "";
    }
}
