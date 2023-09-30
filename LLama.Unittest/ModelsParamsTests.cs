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
                Seed = 42,
                GpuLayerCount = 111,
                LoraAdapters =
                {
                    new("abc", 1),
                    new("def", 0)
                }
            };

            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Converters.Add(new NewtsonsoftEncodingConverter());

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected, settings);
            var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<ModelParams>(json, settings);

            Assert.Equal(expected, actual);
        }



        public class NewtsonsoftEncodingConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(Encoding).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((Encoding)value).WebName);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return Encoding.GetEncoding((string)reader.Value);
            }
        }


    }
}
