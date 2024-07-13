using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLama.Common
{
    internal class PolymorphicJSONConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            string? propertyName = reader.GetString();
            if (propertyName != "Name")
                return default;
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();
            string? name = reader.GetString() ?? throw new JsonException();
            var inheritedTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface
            );
            var type = inheritedTypes.FirstOrDefault(t => t.Name == name);
            if (type == null)
                throw new JsonException();
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            propertyName = reader.GetString();
            if (propertyName != "Data")
                throw new JsonException();
            var data = JsonSerializer.Deserialize(ref reader, type, options);
            if (data == null)
                throw new JsonException();
            reader.Read();
            reader.Read();
            return (T)data;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Name", value.GetType().Name);
            writer.WritePropertyName("Data");
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
            writer.WriteEndObject();
        }
    }
}
