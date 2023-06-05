using Microsoft.Extensions.Options;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Util;

namespace LABTOOLS.API.JsonApi
{
    public class ResourceObjectLinksBuilder
    {
        private readonly AppSettings? _appSettings;
        private readonly IHttpContextAccessor? _accessor;
        private ResourceObjectLinks? _resourceObjectLinks = new ResourceObjectLinks();

        public ResourceObjectLinksBuilder()
        {
            using (var servicScope = ServiceActivator.GetScope())
            {
                var configuration = servicScope.ServiceProvider.GetRequiredService<IConfiguration>();

                _appSettings = new AppSettings(configuration);
                _accessor = servicScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            }
        }

        public ResourceObjectLinks BuildResourceObjectLinks(int id, string type)
        {
            string entityName = type.Substring(0, type.Length - 3) + "s";
            string classType = entityName + "Controller";
            string nameSpace = "LABTOOLS.API.Controllers";

            Type entityType = Type.GetType($"{nameSpace}.{classType}")!;

            if (entityType != null)
            {
                _resourceObjectLinks!.Self = $"{_appSettings!.RequestScheme}://{_appSettings.RequestHost}/api/v{_accessor!.HttpContext!.Request.RouteValues["version"]}/{entityName}/{id}/";
            }
            else
            {
                _resourceObjectLinks!.Self = $"{_appSettings!.RequestScheme}://{_appSettings.RequestHost}/api/v{_accessor!.HttpContext!.Request.RouteValues["version"]}/admin/{entityName}/{id}/";
            }

            return _resourceObjectLinks;
        }
    }
}