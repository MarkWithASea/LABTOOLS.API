using LABTOOLS.API.DataTransferObjects;

namespace LABTOOLS.API.JsonApi
{
    public class ResourceObject<T> : ResourceIdentifierObject
            where T : class, IDataTransferObject
    {
        public ResourceObject(T data)
            : base(data)
        {
            Attributes = data;
            Links = new ResourceObjectLinksBuilder().BuildResourceObjectLinks(data.Id, data.GetType().Name);
        }

        public T Attributes { get; set; }

        public Dictionary<string, RelationshipsObject>? Relationships { get; set; }

        public ResourceObjectLinks? Links { get; set; }
    }
}