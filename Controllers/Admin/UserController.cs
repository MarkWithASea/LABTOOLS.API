using System.Linq.Dynamic.Core;
using AutoMapper;
using Newtonsoft.Json;
using Amazon.CognitoIdentityProvider.Model;
using LABTOOLS.API.CustomAttributes;
using LABTOOLS.API.Data;
using LABTOOLS.API.Data.Repositories;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Extensions;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.JsonApi;
using LABTOOLS.API.Managers;
using LABTOOLS.API.Models;
using LABTOOLS.API.Requests;
using LABTOOLS.API.Requests.Admin;
using LABTOOLS.API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LABTOOLS.API.CustomAttributes.RequiredRoleAttribute;

namespace LABTOOLS.API.Controllers.Admin
{
    [Authorize]
    [ApiController]
    [Route("api/Admin/[controller]")]
    //[RequiredRole(ROLE_ADMIN)]
    public class UserController : AdminApiController<UserDTO, User, AdminUserRepository>
    {
        public UserController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(httpContextAccessor, mapper, configuration)
        { }

        [HttpGet]
        public override async Task<ActionResult<JsonApiDocument<UserDTO>>> Index([FromQuery] IndexQueryParameters queryParams)
        {
            IQueryable<User> query = Repository.GetQuery(UserCognitoId);

            if (queryParams.Filter != null)
            {
                foreach (var filterKvp in queryParams.Filter)
                {
                    var filterProperty = filterKvp.Key;
                    var filterValue = filterKvp.Value;

                    var mappedFilterProperty = QueryParamsMapper.Map(typeof(User), filterProperty);

                    if (mappedFilterProperty.IsNotNullOrWhiteSpace())
                    {
                        query = query.Where(mappedFilterProperty + ".ToLower() == @0", filterValue.ToLower());
                    }
                }
            }

            if (queryParams.Search != null)
            {
                foreach (var searchKvp in queryParams.Search)
                {
                    var searchProperty = searchKvp.Key;
                    var searchValue = searchKvp.Value;

                    var mappedSearchProperty = QueryParamsMapper.Map(typeof(User), searchProperty);

                    if (mappedSearchProperty.IsNotNullOrWhiteSpace())
                    {
                        if (mappedSearchProperty.Split(",").Count() == 1)
                        {
                            query = query.Where(mappedSearchProperty + ".ToLower().Contains(@0)", searchValue.ToLower());
                        }
                        else
                        {
                            var whereClause = string.Join(" || ", mappedSearchProperty.Split(",").Select(x => x + ".ToLower().Contains(@0)"));

                            query = query.Where(whereClause, searchValue.ToLower());
                        }
                    }
                }
            }

            // Execute Query
            var results = await query.ToListAsync();

            if (queryParams.OrderBy?.Keys.Any() == true)
            {
                var sortKvp = queryParams.OrderBy.First();

                var sortProperty = sortKvp.Key.ToLower();
                var sortDirection = sortKvp.Value.ToLower();

                const string SORT_ASC = "asc";

                switch (sortProperty)
                {
                    case "name":
                    case "username":
                        results = (sortDirection == SORT_ASC
                            ? results.OrderBy(r => r.FirstName).ThenBy(r => r.LastName)
                            : results.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName)).ToList();

                        break;
                    case "email":
                        results = (sortDirection == SORT_ASC
                            ? results.OrderBy(r => r.Email)
                            : results.OrderByDescending(r => r.Email)).ToList();

                        break;
                }
            }

            // Get total query count before pagination
            var totalCount = results.Count();

            const string PAGE_NUMBER = "number";
            const string PAGE_SIZE = "size";

            if (queryParams.Page?.ContainsKey(PAGE_NUMBER) == true && queryParams.Page?.ContainsKey(PAGE_SIZE) == true)
            {
                var size = queryParams.Page[PAGE_SIZE];
                var skip = queryParams.Page[PAGE_NUMBER] * size;

                results = results.Skip(skip).Take(size).ToList();
            }

            var dtos = Mapper.Map<IEnumerable<UserDTO>>(results);

            // Get Confirmation Status for each user
            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));

            foreach (var dto in dtos)
            {
                try
                {
                    var user = await client.AdminGetUserAsync(dto.Email!, AppSettings.UserPoolId!);

                    dto.IsConfirmed = user.UserStatus == "CONFIRMED";
                }
                catch (Exception ex)
                {
                    //Logger.Log(LogLevel.Error, ex, $"The user {dto.Email} was not found.");
                }
            }

            Builder.SetData(dtos);
            Builder.SetQueryParams(queryParams);
            Builder.SetTotalCount(totalCount);

            return Builder.GetJsonApiDocument();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Put(int id, object obj)
        {
            RegisterUserRequest model;

            try
            {
                model = JsonConvert.DeserializeObject<RegisterUserRequest>(obj.ToString()!)!;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            User user = await Repository.Get(id);
            if (user == null)
            {
                return NotFound($"No user found with matching Id: {id}");
            }

            var role = await Repository.Context.Roles.FindAsync(model.Data.RoleId);
            if (role == null)
            {
                return NotFound($"No role found with matching Id: {model.Data.RoleId}");
            }

            user.FirstName = model.Data.FirstName;
            user.LastName = model.Data.LastName;
            var oldEmail = user.Email;
            user.Email = model.Data.Email;

            var attributeTypes = new List<AttributeType>();
            attributeTypes.Add(new AttributeType() { Name = "email", Value = model.Data.Email });
            attributeTypes.Add(new AttributeType() { Name = "name", Value = $"{model.Data.FirstName} {model.Data.LastName}" });

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminUpdateUserAttributes(oldEmail, AppSettings.UserPoolId!, AppSettings.AppClientId!, attributeTypes);

            /*
            if (user.Roles!.FirstOrDefault()?.Id != role.Id)
            {
                await ClearUserRoles(user);
                user.Roles!.Add(role);
                await client.AdminAddUserToGroupAsync(user.CognitoId, AppSettings.UserPoolId!, role!.CognitoGroupName);
            }
            */

            await Repository.Save();

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        /*
        [HttpPost("{id}/roles")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> UpdateRoles(int id, AssignRolesRequest model)
        {
            User user = await Repository.Get(id);

            var role = await Repository.Context.Roles.FindAsync(model.Data.RoleId);

            if (role == null)
            {
                return BadRequest();
            }

            await ClearUserRoles(user);

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminAddUserToGroupAsync(user.CognitoId, AppSettings.UserPoolId!, role!.CognitoGroupName);

            await Repository.Save();

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
        */

        [HttpPost("{id}/disable")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Disable(int id)
        {
            User user = await Repository.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsDisabled = true;

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminDisableUserAsync(user.CognitoId, AppSettings.UserPoolId!);

            await Repository.Save();

            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        [HttpPost("{id}/delete")]
        public override async Task<ActionResult<JsonApiDocument<UserDTO>>> Delete(int id)
        {
            var user = await Repository.GetUserToDelete(id);

            if (user is null)
            {
                return NotFound();
            }

            user.IsDeleted = true;
            //user.DeletedDate = DateTime.UtcNow;
            //user.DeletedTransactionId = Guid.NewGuid();

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminDeleteUserAsync(user.CognitoId, AppSettings.UserPoolId!);

            await Repository.Save();

            var context = _appDbContext; //new AppDbContext(_httpContextAccessor, AppSettings.ConnectionString);
            
            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        [HttpPost("{id}/enable")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Enable(int id)
        {
            User user = await Repository.GetDisabledUsers(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsDisabled = false;

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminEnableUserAsync(user.CognitoId, AppSettings.UserPoolId!);

            await Repository.Save();

            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        [HttpPost("register")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Register(RegisterUserRequest model)
        {
            var attributeTypes = new List<AttributeType>();
            attributeTypes.Add(new AttributeType() { Name = "email", Value = model.Data.Email });
            attributeTypes.Add(new AttributeType() { Name = "name", Value = $"{model.Data.FirstName} {model.Data.LastName}" });

            var userWithMatchingEmail = Repository.FindAll(user => user.Email == model.Data.Email && user.IsDeleted == false).FirstOrDefault();
            if (userWithMatchingEmail != null)
            {
                return Conflict("User with that email already exists");
            }

            var role = await Repository.Context.Roles.FindAsync(model.Data.RoleId);
            if (role == null)
            {
                return BadRequest($"Could not find a Role matching Id: {model.Data.RoleId}");
            }

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            AdminCreateUserResponse response = await client.AdminCreateUserAsync(model.Data.Email, AppSettings.UserPoolId!, AppSettings.AppClientId!, attributeTypes);

            var newUser = new User(model.Data.Email, response.User.Username, model.Data.FirstName, model.Data.LastName);
            newUser.Roles!.Add(role);

            await client.AdminAddUserToGroupAsync(newUser.CognitoId, AppSettings.UserPoolId!, role!.CognitoGroupName);

            await Repository.Add(newUser);
            await Repository.Save();

            var dto = Mapper.Map<UserDTO>(newUser);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        [HttpGet("{id}/ResendPassword")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> ResendPassword(int id)
        {
            var user = await Repository.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            var result = await client.AdminResendPasswordAsync(user.Email, AppSettings.UserPoolId!, AppSettings.AppClientId!);

            if ((int)result.HttpStatusCode == 400)
            {
                return StatusCode(455); //PasswordResendError
            }

            var dto = Mapper.Map<UserDTO>(user);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        /*
        private async Task<IEnumerable<string>> ClearUserRoles(User user)
        {
            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            var cognitoUserGroups = await client.AdminListGroupsForUserAsync(user.CognitoId, AppSettings.UserPoolId!);

            var roleGroupNames = cognitoUserGroups.Groups.Select(g => g.GroupName);

            foreach (var groupName in roleGroupNames)
            {
                await client.AdminRemoveUserFromGroupAsync(user.CognitoId, AppSettings.UserPoolId!, groupName);
            }

            user.Roles!.Clear();

            return roleGroupNames;
        }
        */
    }
}