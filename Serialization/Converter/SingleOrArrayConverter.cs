using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LABTOOLS.API.Serialization.Converter
{
    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject(objectType)!;
            }

            Type list = typeof(List<>);
            Type innerType = objectType.GetGenericArguments()[0];
            Type genericList = list.MakeGenericType(innerType);
            var valueArray = new[] { token.ToObject(innerType) };

            object returnObj = Activator.CreateInstance(genericList, valueArray)!;

            return returnObj;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            List<T> list = (List<T>)value!;
            if (list.Count == 1)
            {
                value = list[0];
            }

            serializer.Serialize(writer, value);
        }
    }
}