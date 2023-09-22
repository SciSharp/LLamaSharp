using System.Text;
using EncodingExtensions = LLama.Extensions.EncodingExtensions;

namespace LLama.Unittest
{
    public class EncodingExtensionsTests
    {
        private static void GetCharsTest(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            var chars = new char[128];
            var count = EncodingExtensions.GetCharsImpl(Encoding.UTF8, bytes, chars);

            Assert.Equal(str.Length, count);
            Assert.True(chars[..count].SequenceEqual(str));
        }

        [Fact]
        public void GetCharsEmptyString()
        {
            GetCharsTest("");
        }

        [Fact]
        public void GetCharsString()
        {
            GetCharsTest("Hello World");
        }

        [Fact]
        public void GetCharsChineseString()
        {
            GetCharsTest("猫坐在垫子上");
        }
    }
}
