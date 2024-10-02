using LLama.Common;
using System.Text.Json;
using LLama.Abstractions;

namespace LLama.Unittest
{
    public class ModelsParamsTests
    {
        [Fact]
        public void SerializeRoundTripSystemTextJson()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            var expected = new ModelParams("abc/123")
            {
                BatchSize = 17,
                ContextSize = 42,
                GpuLayerCount = 111,
                TensorSplits = { [0] = 3 },
                MetadataOverrides =
                {
                    new MetadataOverride("hello", true),
                    new MetadataOverride("world", 17),
                    new MetadataOverride("cats", 17f),
                }
            };

            var json = JsonSerializer.Serialize(expected, options);
            var actual = JsonSerializer.Deserialize<ModelParams>(json, options)!;

            // Cannot compare splits with default equality, check they are sequence equal and then set to null
            Assert.True(expected.TensorSplits.SequenceEqual(actual.TensorSplits));
            actual.TensorSplits = null!;
            expected.TensorSplits = null!;

            // Cannot compare overrides with default equality, check they are sequence equal and then set to null
            Assert.True(expected.MetadataOverrides.SequenceEqual(actual.MetadataOverrides));
            actual.MetadataOverrides = null!;
            expected.MetadataOverrides = null!;

            // Check encoding is the same
            var b1 = expected.Encoding.GetBytes("Hello");
            var b2 = actual.Encoding.GetBytes("Hello");
            Assert.True(b1.SequenceEqual(b2));

            Assert.Equal(expected, actual);
        }
    }
}
