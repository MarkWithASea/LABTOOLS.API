using AutoMapper;
using LABTOOLS.API.Data;
using LABTOOLS.API.Data.Repositories;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Models;
using LABTOOLS.API.JsonApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ApiController<UserDTO, User, UserRepository>
    {
        public UserController(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
            : base(appDbContext, httpContextAccessor, mapper, configuration)
        { }

        /*
        public UserApiController(AppDbContext appDbContext, IMapper mapper, IConfiguration configuration)
            : base(appDbContext, mapper, configuration)
        { }
        */

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> GetUser() //int id)
        {
            var results = await Repository.Get(2);
            var dto = Mapper.Map<UserDTO>(results);
            Builder.SetData(dto);
            return Builder.GetJsonApiDocument();
        }
                
        [HttpGet]
        public async Task<ActionResult<JsonApiDocument<UserDTO>>> GetQuery()
        {
            IQueryable<User> query = Repository.GetQuery(UserCognitoId);

            var results = await query.ToListAsync();
            var count = results.Count();

            var dtos = Mapper.Map<IEnumerable<UserDTO>>(results).FirstOrDefault()!;           

            Builder.SetData(dtos);
            return Builder.GetJsonApiDocument();
        }
    }
}