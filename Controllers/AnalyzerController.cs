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
    public class AnalyzerController : ApiController<AnalyzerDTO, Models.Analyzer, AnalyzerRepository>
    {
        public AnalyzerController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(httpContextAccessor, mapper, configuration)
        { }

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonApiDocument<AnalyzerDTO>>> GetAnalyzer(int id)
        {
            var results = await Repository.Get(id);
            var dto = Mapper.Map<AnalyzerDTO>(results);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
                
        [HttpGet]
        public async Task<ActionResult<JsonApiDocument<AnalyzerDTO>>> Index([FromQuery] IndexQueryParameters queryParams)
        {
            IQueryable<Models.Analyzer> query = Repository.GetQuery(UserCognitoId);

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
                            results = orderByValue == "asc" 
                            ? results.OrderBy(r => r.Name).ToList()
                            : results.OrderByDescending(r => r.Name).ToList();
                            break;
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

            var dtos = Mapper.Map<IEnumerable<AnalyzerDTO>>(results);

            Builder.SetData(dtos);
            Builder.SetQueryParams(queryParams);
            Builder.SetTotalCount(count);
            return Builder.GetJsonApiDocument();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<JsonApiDocument<AnalyzerDTO>>> Put(int id, object obj)
        {
            AnalyzerRequest model;

            try
            {
                model = JsonConvert.DeserializeObject<AnalyzerRequest>(obj.ToString()!)!;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            Models.Analyzer analyzer = await Repository.Get(id);
            if (analyzer == null)
            {
                return NotFound($"No user found with matching Id: {id}");
            }

            analyzer.Name = model.Data.Name;
            
            await Repository.Save();

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<AnalyzerDTO>(analyzer);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
    }
}