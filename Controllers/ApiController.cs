using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LABTOOLS.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiController<TDto, TEntity> : ControllerBase
        where TDto : class, IDataTransferOjbect
        where TEntity : class, IEntity
    {
        private AppSettings _AppSettings;
        //private User? _sessionUser;
        //protected IHttpContextAccessor _httpContextAccessor;

        protected AppSettings AppSettings {get {return _AppSettings; } }
        //protected User? SessionUser { get {return _sessionUser;} }
        
        public ApiController(IConfiguration configuration) //IHttpContextAccessor httpContextAccessor, 
        {
            //_httpContextAccessor = httpContextAccessor;
            _AppSettings = new AppSettings(configuration);
        }
    }
}