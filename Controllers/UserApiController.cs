using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LABTOOLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ApiController<UserDTO, User>
    {
        public UserApiController(IConfiguration configuration)
            : base(configuration)
        { }

    }
}