using Newtonsoft.Json;

namespace PrimeTime.Models
{
    internal class StrictIntConverter : JsonConverter
    {
        readonly JsonSerializer defaultSerializer = new JsonSerializer();

        public override bool CanConvert(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(long)
                || type == typeof(double))
                return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return reader.TokenType switch
            {
                JsonToken.Integer or JsonToken.Float => defaultSerializer.Deserialize(reader, objectType),
                _ => throw new JsonSerializationException(string.Format("Token \"{0}\" of type {1} was not a JSON integer", reader.Value, reader.TokenType)),
            };
#pragma warning restore CS8603 // Possible null reference return.
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
