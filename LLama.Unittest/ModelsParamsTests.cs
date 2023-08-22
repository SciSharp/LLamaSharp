using LLama.Common;

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
                GroupedQueryAttention = 7,
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
                GroupedQueryAttention = 7,
                Seed = 42,
                GpuLayerCount = 111
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected);
            var actual = Newtonsoft.Json.JsonConvert.DeserializeObject<ModelParams>(json);

            Assert.Equal(expected, actual);
        }
    }
}
