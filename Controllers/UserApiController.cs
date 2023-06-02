using AutoMapper;
using LABTOOLS.API.Data;
using LABTOOLS.API.Data.Repositories;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ApiController<UserDTO, User, UserRepository>
    {
        //public UserApiController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
        //    : base(httpContextAccessor, mapper, configuration)
        //{ }

        public UserApiController(AppDbContext appDbContext, IMapper mapper, IConfiguration configuration)
            : base(appDbContext, mapper, configuration)
        { }

        /*
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var results = await Repository.Get(id);
            var dto = Mapper.Map<UserDTO>(results);
            return dto;
        }
        */

        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetQuery(string userCognitoId)
        {
            IQueryable<User> query = Repository.GetQuery(userCognitoId);

            var results = await query.ToListAsync();
            
            var dtos = Mapper.Map<IEnumerable<UserDTO>>(results).FirstOrDefault()!;

            //Builder

            return dtos;
        }
    }
}