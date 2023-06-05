namespace LABTOOLS.API.JsonApi
{
    public class RelationshipsObject
    {
        public Dictionary<string, string>? Links { get; set; }

        public List<ResourceIdentifierObject>? Data { get; set; }

        public object? Meta { get; set; }
    }
}