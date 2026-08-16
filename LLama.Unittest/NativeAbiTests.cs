using LLama.Native;
using System.Runtime.InteropServices;

namespace LLama.Unittest
{
    public class NativeAbiTests
    {
        [Fact]
        public void ModelMetadataOverrideLayoutMatchesNative()
        {
            Assert.Equal(0, Marshal.OffsetOf<LLamaModelMetadataOverride>("Tag").ToInt32());
            Assert.Equal(4, Marshal.OffsetOf<LLamaModelMetadataOverride>("key").ToInt32());
            Assert.Equal(136, Marshal.OffsetOf<LLamaModelMetadataOverride>("IntValue").ToInt32());
            Assert.Equal(264, Marshal.SizeOf<LLamaModelMetadataOverride>());
        }

        [Fact]
        public void TokenDataArrayLayoutMatchesNative()
        {
            var pointerSize = IntPtr.Size;

            Assert.Equal(0, Marshal.OffsetOf<LLamaTokenDataArrayNative>("_data").ToInt32());
            Assert.Equal(pointerSize, Marshal.OffsetOf<LLamaTokenDataArrayNative>("_size").ToInt32());

            var selectedOffset = Align(pointerSize + pointerSize, 8);
            Assert.Equal(selectedOffset, Marshal.OffsetOf<LLamaTokenDataArrayNative>("_selected").ToInt32());

            var sortedOffset = selectedOffset + sizeof(long);
            Assert.Equal(sortedOffset, Marshal.OffsetOf<LLamaTokenDataArrayNative>("_sorted").ToInt32());

            var expectedSize = Align(sortedOffset + sizeof(sbyte), 8);
            Assert.Equal(expectedSize, Marshal.SizeOf<LLamaTokenDataArrayNative>());
        }

        [Fact]
        public void SamplerInterfaceSizeMatchesNative()
        {
            var expected = IntPtr.Size * 10;
            Assert.Equal(expected, Marshal.SizeOf<LLamaSamplerINative>());
        }

        private static int Align(int value, int alignment)
        {
            return (value + alignment - 1) / alignment * alignment;
        }

        private static int ComputeSize(IEnumerable<(int size, int align)> fields)
        {
            var offset = 0;
            var maxAlignment = 1;

            foreach (var field in fields)
            {
                maxAlignment = Math.Max(maxAlignment, field.align);
                offset = Align(offset, field.align);
                offset += field.size;
            }

            return Align(offset, maxAlignment);
        }
    }
}
