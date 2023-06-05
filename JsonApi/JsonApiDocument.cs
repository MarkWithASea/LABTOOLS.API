using Newtonsoft.Json;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Serialization.Converter;

namespace LABTOOLS.API.JsonApi
{
    public class JsonApiDocument<T> : DocumentBase
         where T : class, IDataTransferObject
    {
        public JsonApiDocument()
        { }

        public JsonApiDocument(T data)
        {
            Data = new List<ResourceObject<T>>();
            Data.Add(new ResourceObject<T>(data));
        }

        public JsonApiDocument(IEnumerable<T> data)
        {
            Data = data.Select(d => new ResourceObject<T>(d)).ToList();
        }

        [JsonConverter(typeof(SingleOrArrayConverter<ResourceObject<IDataTransferObject>>))]
        public List<ResourceObject<T>>? Data { get; set; }

        public List<object>? Included { get; set; }
    }
}