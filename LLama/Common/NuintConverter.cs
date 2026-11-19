using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLama.Common
{
    /// <summary>
    /// A JSON converter for <see cref="nuint"/>, which System.Text.Json does not support natively.
    /// Values are written as unsigned 64-bit numbers.
    /// </summary>
    public class NuintConverter
        : JsonConverter<nuint>
    {
        /// <inheritdoc/>
        public override nuint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetUInt64();
            if (value > nuint.MaxValue)
                throw new JsonException($"Value {value} does not fit into nuint on this platform.");
            return (nuint)value;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, nuint value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((ulong)value);
        }
    }
}
