using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LABTOOLS.API.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiredRoleAttribute : Attribute, IAuthorizationFilter
    {
        public RequiredRoleAttribute(params string[] roleNames)
        {
            _roleNames = new List<string>();

            Debug.Assert(roleNames?.Length > 0, $"{nameof(roleNames)} must contain at least one role.");

            if (roleNames != null)
            {
                _roleNames.AddRange(roleNames);
            }
        }

        // TODO: determine common location for these constants, so that
        // they may be referenced when declaring and examining roles
        public const string ROLE_PREFIX = "LABTOOLS:Role:";
        public const string GROUPS_TYPE = "cognito:groups";
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_MANAGER = "Manager";
        public const string ROLE_USER = "User";

        private readonly List<string> _roleNames;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
            {
                return;
            }

            // authorization
            if (!(context.HttpContext.User is ClaimsPrincipal user))
            {
                // user not found
                context.Result = new UnauthorizedResult();

                return;
            }

            var userRoles = user.Claims.Where(c => c.Type == GROUPS_TYPE
                                                && c.Value.StartsWith(ROLE_PREFIX))
                                       .Select(c => c.Value.Replace(ROLE_PREFIX, null))
                                       .ToList();

            var isAuthorized = userRoles.Intersect(_roleNames).Any();

            if (!isAuthorized)
            {
                // not logged in or role not authorized
                context.Result = new UnauthorizedResult();
            }
        }
    }
}