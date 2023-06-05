using LABTOOLS.API.Requests;

namespace LABTOOLS.API.JsonApi
{
    public static class MetaBuilder
    {
        public static TopLevelMeta BuildTopLevelMeta(int totalCount)
        {
            var meta = new TopLevelMeta();

            meta.Total = totalCount;

            return meta;
        }

        public static TopLevelMeta BuildTopLevelMeta(IndexQueryParameters queryParams, int totalCount)
        {
            var meta = new TopLevelMeta();

            meta.Total = totalCount;
            meta.PageNumber = queryParams.Page?["number"];
            meta.PageSize = queryParams.Page?["size"];

            return meta;
        }
    }
}