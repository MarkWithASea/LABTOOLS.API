using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Requests;
using LABTOOLS.API.Util;

namespace LABTOOLS.API.JsonApi
{
    public class JsonApiDocBuilder<TDto> : IJsonApiDocBuilder<TDto>
        where TDto : class, IDataTransferObject
    {
        private AppSettings _appSettings;

        private IHttpContextAccessor _accessor;

        private LinkBuilder _linkBuilder;

        private JsonApiDocument<TDto> _jsonApiDocument = new JsonApiDocument<TDto>();

        private IndexQueryParameters? _queryParams;

        private int _totalCount;

        public JsonApiDocBuilder(AppSettings appSettings)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                _accessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                _linkBuilder = serviceScope.ServiceProvider.GetRequiredService<LinkBuilder>();
            }

            _appSettings = appSettings;

            this.Reset();
        }

        public void Reset()
        {
            _jsonApiDocument = new JsonApiDocument<TDto>();
        }

        public void SetData(TDto data)
        {
            _jsonApiDocument.Data = new List<ResourceObject<TDto>>();
            _jsonApiDocument.Data.Add(new ResourceObject<TDto>(data));
        }

        public void SetData(IEnumerable<TDto> data)
        {
            _jsonApiDocument.Data = data.Select(d => new ResourceObject<TDto>(d)).ToList();
        }

        public void SetQueryParams(IndexQueryParameters queryParams)
        {
            _queryParams = queryParams;
        }

        public void SetTotalCount(int totalCount)
        {
            _totalCount = totalCount;
        }

        public void SetTopLevelLinks()
        {
            _jsonApiDocument.Links = _queryParams != null ? _linkBuilder!.BuildTopLevelLinks(_queryParams, _totalCount) : _linkBuilder!.BuildTopLevelLinks();
        }

        public void SetMetaData()
        {
            if (_totalCount == 0)
            {
                _totalCount = _jsonApiDocument.Data!.Count();
            }

            _jsonApiDocument.Meta = _queryParams != null ? MetaBuilder.BuildTopLevelMeta(_queryParams!, _totalCount) : MetaBuilder.BuildTopLevelMeta(_totalCount);
        }

        public JsonApiDocument<TDto> GetJsonApiDocument()
        {
            SetTopLevelLinks();
            SetMetaData();

            JsonApiDocument<TDto> result = _jsonApiDocument;

            Reset();

            return result;
        }
    }
}