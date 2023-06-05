using LABTOOLS.API.DataTransferObjects;

namespace LABTOOLS.API.JsonApi
{
    public class ResourceIdentifierObject
    {
        public ResourceIdentifierObject(IDataTransferObject data)
        {
            Id = data.Id;
            // TODO: Create method in interface to deal with this so "DTO" doesn't get exposed
            Type = data.GetType().Name;
        }

        public int Id { get; set; }

        public string Type { get; set; }

        public object? Meta { get; set; }
    }
}