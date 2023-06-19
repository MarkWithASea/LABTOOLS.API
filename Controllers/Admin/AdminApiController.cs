using System.Linq.Dynamic.Core;
using AutoMapper;
using LABTOOLS.API.Data;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.JsonApi;
using LABTOOLS.API.Models;
using LABTOOLS.API.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]

    public abstract class AdminApiController<TDto, TEntity, TRepository> : ApiController<TDto, TEntity, TRepository>
     where TDto : class, IDataTransferObject
     where TEntity : class, IEntity
     where TRepository : IRepository<TEntity>
    {
        public AdminApiController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(httpContextAccessor, mapper, configuration)
        { }

        [HttpGet]
        public virtual async Task<ActionResult<JsonApiDocument<TDto>>> Index([FromQuery] IndexQueryParameters queryParams)
        {
            IQueryable<TEntity> query = Repository.GetQuery(UserCognitoId);

            //FILTERS
            //SEARCH
            //ORDER BY

            var count = query.Count();

            if (queryParams.Page?.ContainsKey("number") == true && queryParams.Page?.ContainsKey("size") == true)
            {
                var skip = queryParams.Page["number"] * queryParams.Page["size"];
                query = query.Skip(skip).Take(queryParams.Page["size"]);
            }

            var results = await query.ToListAsync();

            var dtos = Mapper.Map<IEnumerable<TDto>>(results);

            Builder.SetData(dtos);
            Builder.SetQueryParams(queryParams);
            Builder.SetTotalCount(count);
            return Builder.GetJsonApiDocument();
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<JsonApiDocument<TDto>>> Get(int id)
        {
            TEntity result = await Repository.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<TDto>(result);

            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiDocument<TDto>>> JsonPatchWithModelState(int id, [FromBody] JsonPatchDocument<TEntity> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var entity = await Repository.Get(id);

            patchDoc.ApplyTo(entity, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Repository.Save();

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<TDto>(entity);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<JsonApiDocument<TDto>>> Delete(int id)
        {
            var result = await Repository.Delete(id);
            if (result == null)
            {
                return NotFound();
            }

            // Build JsonApiDocument of data transfer object
            var dto = Mapper.Map<TDto>(result);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
    }
}