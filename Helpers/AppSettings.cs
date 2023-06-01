namespace LABTOOLS.API.Helpers
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IConfiguration _configuration;

        public string? AppClientId {get { return _configuration[nameof(AppClientId)]; } }

        public string? ConnectionString { get { return _configuration[nameof(ConnectionString)]; } }
        
        public string? Region { get { return _configuration[nameof(Region)]; } }

        public string? UserPoolId { get { return _configuration[nameof(UserPoolId)]; } }
    }   
}