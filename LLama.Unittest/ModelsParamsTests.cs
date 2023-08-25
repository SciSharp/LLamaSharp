using System.Text;
using LLama.Common;
using Newtonsoft.Json;

namespace LLama.Unittest
{
    public class ModelsParamsTests
    {
        [Fact]
        public void SerializeRoundTripSystemTextJson()
        {
            var expected = new ModelParams("abc/123")
            {
                BatchSize = 17,
                ContextSize = 42,
                LoraAdapter = "adapter",
                Seed = 42,
                GpuLayerCount = 111
            };

            var json = System.Text.Json.JsonSerializer.Serialize(expected);
            var actual = System.Text.Json.JsonSerializer.Deserialize<ModelParams>(json);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SerializeRoundTripNewtonsoft()
        {
            var expected = new ModelParams("abc/123")
            {
                BatchSize = 17,
                ContextSize = 42,
                LoraAdapter = "adapter",
                Seed = 42,
                GpuLayerCount = 111
            };

            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Converters.Add(new NewtsonsoftEncodingConverter());

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected, settings);
            var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<ModelParams>(json, settings);

            Assert.Equal(expected, actual);
        }

        private class NewtsonsoftEncodingConverter
            : Newtonsoft.Json.JsonConverter<Encoding>
        {
            public override void WriteJson(JsonWriter writer, Encoding? value, JsonSerializer serializer)
            {
                writer.WriteValue((string?)value?.WebName);
            }

            public override Encoding? ReadJson(JsonReader reader, Type objectType, Encoding? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var name = (string?)reader.Value;
                if (name == null)
                    return null;
                return Encoding.GetEncoding(name);
            }
        }
    }
}
