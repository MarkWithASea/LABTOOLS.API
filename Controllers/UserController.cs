using System.Net;
using System.Linq.Dynamic.Core;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using LABTOOLS.API.Data;
using LABTOOLS.API.Data.Repositories;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.JsonApi;
using LABTOOLS.API.Managers;
using LABTOOLS.API.Models;
using LABTOOLS.API.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LABTOOLS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ApiController<UserDTO, User, UserRepository>
    {
        public UserController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(httpContextAccessor, mapper, configuration)
        { }

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> GetUser(int id)
        {
            var results = await Repository.Get(id);
            var dto = Mapper.Map<UserDTO>(results);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
                
        [HttpGet]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Index([FromQuery] IndexQueryParameters queryParams)
        {
            IQueryable<User> query = Repository.GetQuery(UserCognitoId);

            if (queryParams.Filter != null)
            {
                foreach(var filter in queryParams.Filter)
                {
                    var filterKey = filter.Key;
                    var filterValue = filter.Value;

                    query = query.Where($"{filterKey}.ToLower() == @0", filterValue.ToLower());
                }
            }

            if (queryParams.Search != null)
            {
                foreach (var search in queryParams.Search)
                {
                    var searchKey = search.Key;
                    var searchValue = search.Value;

                    if (searchKey.Split(",").Count() == 1)
                    {
                        query = query.Where($"{searchKey}.ToLower().Contains(@0)", searchValue.ToLower());
                    }
                    else
                    {
                        var where = string.Join(" || ", searchKey.Split(",").Select( x => $"{x}.ToLower().Contains(@0)"));
                        query = query.Where(where, searchValue.ToLower());
                    }
                }
            }

            var results = await query.ToListAsync();

            if (queryParams.OrderBy != null && queryParams.OrderBy.Keys.Any())
            {
                foreach(var orderBy in queryParams.OrderBy.Reverse())
                {
                    var orderByKey = orderBy.Key.ToLower();
                    var orderByValue = orderBy.Value.ToLower();

                    switch(orderByKey)
                    {
                        case "name":
                        case "username":
                            results = orderByValue == "asc" 
                            ? results.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToList()
                            : results.OrderByDescending(r => r.LastName).ThenByDescending(r => r.FirstName).ToList();
                            break;
                        case "email":
                            results = orderByValue == "asc" 
                            ? results.OrderBy(r => r.Email).ToList()
                            : results.OrderByDescending(r => r.Email).ToList();
                            break;
                        //case "role":
                        //    break;
                    }
                }
            }

            if (queryParams.Page != null && queryParams.Page.ContainsKey("number") && queryParams.Page.ContainsKey("size"))
            {
                var size = queryParams.Page["size"];
                var skip = queryParams.Page["number"] * size;

                results = results.Skip(skip).Take(size).ToList();
            }

            var count = results.Count();

            var dtos = Mapper.Map<IEnumerable<UserDTO>>(results);

            Builder.SetData(dtos);
            Builder.SetQueryParams(queryParams);
            Builder.SetTotalCount(count);
            return Builder.GetJsonApiDocument();
        }

        [HttpPut]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> Put(UpdateUserRequest obj)
        {
            SessionUser.FirstName = obj.Data.FirstName;
            SessionUser.LastName = obj.Data.LastName;

            var attributeTypes = new List<AttributeType>();
            attributeTypes.Add(new AttributeType() { Name = "name", Value = $"{obj.Data.FirstName} {obj.Data.LastName}" });

            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            await client.AdminUpdateUserAttributes(SessionUser.Email, AppSettings.UserPoolId!, AppSettings.AppClientId!, attributeTypes);

            await Repository.Save();

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<UserDTO>(SessionUser);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
        
        [HttpPost("password")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> UpdatePassword([FromBody] UpdateUserPasswordRequest request)
        {
            var client = new CognitoUserManager(Amazon.RegionEndpoint.GetBySystemName(AppSettings.Region));
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ChangePasswordResponse response;

            try
            {
                response = await client.ChangePasswordAsync(accessToken!, request.Data.OldPassword, request.Data.NewPassword);
            }
            catch (NotAuthorizedException ex)
            {
                return StatusCode((int)HttpStatusCode.NotAcceptable);
            }
            catch (LimitExceededException ex)
            {
                return StatusCode((int)HttpStatusCode.TooManyRequests);
            }

            if (response.HttpStatusCode.GetHashCode() != StatusCodes.Status200OK)
            {
                return BadRequest();
            }

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<UserDTO>(SessionUser);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
    }
}