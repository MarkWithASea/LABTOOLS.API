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
    public class RoleController : AdminApiController<RoleDTO, Role, AdminRoleRepository>
    {
        public RoleController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(httpContextAccessor, mapper, configuration)
        { }

        [HttpGet]
        public override async Task<ActionResult<JsonApiDocument<RoleDTO>>> Index([FromQuery] IndexQueryParameters queryParams)
        {
            IQueryable<Role> query = Repository.GetQuery(UserCognitoId);

            if (queryParams.Filter != null)
            {
                foreach (var filterKvp in queryParams.Filter)
                {
                    var filterProperty = filterKvp.Key;
                    var filterValue = filterKvp.Value;

                    var mappedFilterProperty = QueryParamsMapper.Map(typeof(Role), filterProperty);

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
                        results = (sortDirection == SORT_ASC
                            ? results.OrderBy(r => r.Name)
                            : results.OrderByDescending(r => r.Name)).ToList();

                        break;
                    case "description":
                        results = (sortDirection == SORT_ASC
                            ? results.OrderBy(r => r.Description)
                            : results.OrderByDescending(r => r.Description)).ToList();

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

            var dtos = Mapper.Map<IEnumerable<RoleDTO>>(results);

            Builder.SetData(dtos);
            Builder.SetQueryParams(queryParams);
            Builder.SetTotalCount(totalCount);

            return Builder.GetJsonApiDocument();
        }
    }
}