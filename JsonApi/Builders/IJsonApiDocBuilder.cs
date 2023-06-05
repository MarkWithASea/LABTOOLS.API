using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Requests;

namespace LABTOOLS.API.JsonApi
{
    public interface IJsonApiDocBuilder<TDto>
        where TDto : class, IDataTransferObject
    {
        void Reset();

        void SetData(TDto data);

        void SetData(IEnumerable<TDto> data);

        void SetQueryParams(IndexQueryParameters queryParams);

        void SetTotalCount(int totalCount);

        void SetTopLevelLinks();

        void SetMetaData();

        JsonApiDocument<TDto> GetJsonApiDocument();
    }
}