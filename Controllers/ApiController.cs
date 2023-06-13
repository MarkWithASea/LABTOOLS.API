using AutoMapper;
using LABTOOLS.API.Data;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.JsonApi;
using LABTOOLS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LABTOOLS.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiController<TDto, TEntity, TRepository> : ControllerBase
        where TDto : class, IDataTransferObject
        where TEntity : class, IEntity
        where TRepository : IRepository<TEntity>
    {
        private TRepository? _repository;
        private IMapper _mapper;
        private User? _sessionUser;
        private IJsonApiDocBuilder<TDto> _builder;
        
        protected AppDbContext _appDbContext;
        
        protected IHttpContextAccessor _httpContextAccessor;
        private AppSettings _appSettings;

        protected AppSettings AppSettings
        {
            get
            {
                return _appSettings;
            }
        }

        protected AppDbContext AppDbContext
        {
            get
            {
                return _appDbContext;
            }
        }

        protected IMapper Mapper
        {
            get
            {
                return _mapper;
            }
        }
        
        protected IJsonApiDocBuilder<TDto> Builder
        {
            get
            {
                return _builder;
            }
        }
        
        protected TRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = (TRepository)Activator.CreateInstance(typeof(TRepository), new object[] {_httpContextAccessor, AppSettings})!;
                }

                return _repository;
            }
        }

        protected User? SessionUser
        {
            get
            {
                if (_sessionUser == null)
                {
                    _sessionUser = Repository.GetUser(UserCognitoId).Result;
                }
                
                return _sessionUser;
            }
        }

        protected string UserCognitoId
        {
            get
            {
                return User.Claims.FirstOrDefault(c => c.Type == "username")!.Value;
            }
        }

        /*
        public ApiController(AppDbContext appDbContext, IMapper mapper, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _appSettings = new AppSettings(configuration);
            _builder = new JsonApiDocBuilder<TDto>(_appSettings);
        }
        */
        
        public ApiController(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _appSettings = new AppSettings(configuration);
            _builder = new JsonApiDocBuilder<TDto>(_appSettings);
        }
    }
}