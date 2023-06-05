using LABTOOLS.API.Helpers;
using LABTOOLS.API.Requests;
using LABTOOLS.API.Extensions;
using Microsoft.Extensions.Options;

namespace LABTOOLS.API.JsonApi
{
    public class LinkBuilder
    {
        private readonly AppSettings? _appSettings;
        private readonly IHttpContextAccessor? _accessor;
        private string _baseUrl;

        public LinkBuilder(IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _appSettings = new AppSettings(configuration);
            _accessor = accessor;
            _baseUrl = $"{_appSettings!.RequestScheme}://{_appSettings.RequestHost}{_accessor!.HttpContext!.Request.Path}";
        }

        // Build links for endpoints without pagination
        public TopLevelLinks BuildTopLevelLinks()
        {
            var links = new TopLevelLinks();

            links.Self = _baseUrl;

            return links;
        }

        // Build links for endpoints with pagination
        public TopLevelLinks BuildTopLevelLinks(IndexQueryParameters queryParams, int totalCount)
        {
            var links = new TopLevelLinks();

            // Build query strings for links
            string queryStringSelf = BuildSelfQueryString(queryParams);
            string queryStringPrev = BuildPrevQueryString(queryParams)!;
            string queryStringNext = BuildNextQueryString(queryParams, totalCount)!;
            string queryStringLast = BuildLastQueryString(queryParams, totalCount);

            // Format links for the JSON:API top level links object
            links.Self = $"{_baseUrl}{queryStringSelf}";
            links.Prev = !queryStringPrev.IsNotNullOrEmpty() ? null : $"{_baseUrl}{queryStringPrev}";
            links.Next = !queryStringNext.IsNotNullOrEmpty() ? null : $"{_baseUrl}{queryStringNext}";
            links.Last = !queryStringLast.IsNotNullOrEmpty() ? null : $"{_baseUrl}{queryStringLast}";

            return links;
        }

        public string BuildSelfQueryString(IndexQueryParameters queryParams)
        {
            string? queryStringSelf = null;
            string? pageNumber = null;
            string? pageSize = null;
            string? filtersQueryString = null;

            if (queryParams!.Page!.IsNotNullOrEmpty())
            {
                pageNumber = "?page[number]=" + queryParams!.Page!["number"];
                pageSize = "&page[size]=" + queryParams.Page["size"];
            }

            filtersQueryString = BuildFiltersQueryString(queryParams);

            return queryStringSelf = pageNumber + pageSize + filtersQueryString;
        }

        public string? BuildPrevQueryString(IndexQueryParameters queryParams)
        {
            string? queryStringPrev = null;
            string? pageNumber = null;
            string? pageSize = null;
            int prevPage = 0;
            string? filtersQueryString = null;

            if (queryParams!.Page!.IsNotNullOrEmpty())
            {
                if (queryParams!.Page!["number"] > 0)
                {
                    prevPage = queryParams.Page!["number"] - 1;
                }
                else
                {
                    return null;
                }

                pageNumber = "?page[number]=" + prevPage;
                pageSize = "&page[size]=" + queryParams.Page["size"];
            }

            filtersQueryString = BuildFiltersQueryString(queryParams);

            return queryStringPrev = pageNumber + pageSize + filtersQueryString;
        }

        public string? BuildNextQueryString(IndexQueryParameters queryParams, int totalCount)
        {
            string? queryStringNext = null;
            string? pageNumber = null;
            string? pageSize = null;
            double nextPage = 0;
            string? filtersQueryString = null;

            if (queryParams!.Page!.IsNotNullOrEmpty())
            {
                double size = queryParams.Page!["size"];
                double count = (double)totalCount;
                double pageCount = Math.Ceiling(totalCount / size);

                if (totalCount > queryParams.Page!["size"])
                {
                    nextPage = queryParams.Page!["number"] + 1;
                    if (nextPage >= pageCount)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

                pageNumber = "?page[number]=" + nextPage;
                pageSize = "&page[size]=" + queryParams.Page["size"];
            }

            filtersQueryString = BuildFiltersQueryString(queryParams);

            return queryStringNext = pageNumber + pageSize + filtersQueryString;
        }

        public string BuildLastQueryString(IndexQueryParameters queryParams, int totalCount)
        {
            string? queryStringLast = null;
            string? pageNumber = null;
            string? pageSize = null;
            int lastPage = 0;
            string? filtersQueryString = null;

            if (queryParams!.Page!.IsNotNullOrEmpty())
            {
                if (totalCount == queryParams.Page!["size"])
                {
                    lastPage = 0;
                }
                else
                {
                    lastPage = totalCount / queryParams.Page!["size"];
                }

                pageNumber = "?page[number]=" + lastPage;
                pageSize = "&page[size]=" + queryParams.Page["size"];
            }

            filtersQueryString = BuildFiltersQueryString(queryParams);

            return queryStringLast = pageNumber + pageSize + filtersQueryString;
        }

        // Build query string portion for Order, Filter, and Search
        public string BuildFiltersQueryString(IndexQueryParameters queryParams)
        {
            string? filtersQueryString = null;
            string? orderBy = null;
            string? filterBy = null;
            string? searchBy = null;

            if (queryParams.OrderBy!.IsNotNullOrEmpty())
            {
                orderBy = "&orderBy[" + queryParams.OrderBy!.FirstOrDefault().Key + "]=" + queryParams.OrderBy!.FirstOrDefault().Value;
            }

            if (queryParams.Filter!.IsNotNullOrEmpty())
            {
                filterBy = "&filter[" + queryParams.Filter!.FirstOrDefault().Key + "]=" + queryParams.Filter!.FirstOrDefault().Value;
            }

            if (queryParams.Search!.IsNotNullOrEmpty())
            {
                searchBy = "&search[" + queryParams.Search!.FirstOrDefault().Key + "]=" + queryParams.Search!.FirstOrDefault().Value;
            }

            return filtersQueryString = orderBy + filterBy + searchBy;
        }
    }
}