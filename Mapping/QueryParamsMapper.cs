using LABTOOLS.API.Models;

namespace LABTOOLS.API.Mapping
{
    public static class QueryParamsMapper
    {
        public static string? Map(Type entityType, string queryParams)
        {
            var mapping = new Dictionary<string, string>();

            if (entityType == typeof(User))
            {
                mapping = new Dictionary<string, string>();
                
            }

            string? mappedValue = null;
            if (mapping.ContainsKey(queryParams.ToLower()))
            {
                mappedValue = mapping[queryParams].ToLower();
            }

            return mappedValue;
        }
    }
}