namespace LABTOOLS.API.Helpers
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IConfiguration _configuration;

        public string? AllowedOrigins { get { return _configuration[nameof(AllowedOrigins)]; } }
        
        public string? AppClientId {get { return _configuration[nameof(AppClientId)]; } }

        public string ConnectionString { get { return _configuration[nameof(ConnectionString)]; } }
        
        public string? Region { get { return _configuration[nameof(Region)]; } }

        public string? UserPoolId { get { return _configuration[nameof(UserPoolId)]; } }
        
        public string? RequestHost { get { return _configuration[nameof(RequestHost)]; } }

        public string? RequestScheme { get { return _configuration[nameof(RequestScheme)]; } }
    }   
}