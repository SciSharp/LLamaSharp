using LLama.Extensions;

namespace LLama.Unittest
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetDefaultValueEmptyDict()
        {
            var dict = new Dictionary<int, int>();

            Assert.Equal(42, DictionaryExtensions.GetValueOrDefaultImpl(dict, 0, 42));
        }

        [Fact]
        public void GetDefaultValueMissingKey()
        {
            var dict = new Dictionary<int, int>()
            {
                { 3, 4 }
            };

            Assert.Equal(43, DictionaryExtensions.GetValueOrDefaultImpl(dict, 0, 43));
        }

        [Fact]
        public void GetValue()
        {
            var dict = new Dictionary<int, int>()
            {
                { 3, 4 },
                { 4, 5 },
            };

            Assert.Equal(4, DictionaryExtensions.GetValueOrDefaultImpl(dict, 3, 42));
        }
    }
}
