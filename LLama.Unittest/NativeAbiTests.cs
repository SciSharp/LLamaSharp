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
            var expected = IntPtr.Size * 12;
            Assert.Equal(expected, Marshal.SizeOf<LLamaSamplerINative>());
        }

        /// <summary>
        /// Check sizes measured from the v0.9.0 headers on 64-bit and 32-bit builds.
        /// </summary>
        [Theory]
        [InlineData(typeof(LLamaModelParams), 80, 52)]
        [InlineData(typeof(LLamaContextParams), 168, 136)]
        [InlineData(typeof(LLamaModelQuantizeParams), 64, 44)]
        [InlineData(typeof(LLamaPerfContextTimings), 56, 56)]
        [InlineData(typeof(NativeApi.mtmd_context_params), 96, 60)]
        [InlineData(typeof(NativeApi.mtmd_helper_init_opt), 32, 24)]
        [InlineData(typeof(NativeApi.mtmd_helper_video_init_params), 32, 24)]
        public void ParameterSizesMatchNative(Type type, int size64, int size32)
        {
            Assert.Equal(IntPtr.Size == 8 ? size64 : size32, Marshal.SizeOf(type));
        }

        /// <summary>
        /// Check fields whose offsets changed in the native ABI.
        /// </summary>
        [Theory]
        [InlineData(typeof(LLamaModelParams), "lazy_mode", 28, 20)]
        [InlineData(typeof(LLamaModelParams), "main_gpu", 32, 24)]
        [InlineData(typeof(LLamaModelParams), "tensor_split", 40, 28)]
        [InlineData(typeof(LLamaContextParams), "n_outputs_max_per_seq", 24, 24)]
        [InlineData(typeof(LLamaContextParams), "n_threads", 28, 28)]
        [InlineData(typeof(LLamaContextParams), "moe_cache_size", 112, 104)]
        [InlineData(typeof(LLamaContextParams), "abort_callback", 120, 108)]
        [InlineData(typeof(LLamaContextParams), "_embeddings", 136, 116)]
        [InlineData(typeof(LLamaContextParams), "samplers", 144, 124)]
        [InlineData(typeof(LLamaContextParams), "ctx_other", 160, 132)]
        [InlineData(typeof(LLamaModelQuantizeParams), "max_buf_size", 56, 40)]
        [InlineData(typeof(LLamaPerfContextTimings), "n_reallocs_same", 44, 44)]
        [InlineData(typeof(LLamaPerfContextTimings), "n_reallocs_diff", 48, 48)]
        [InlineData(typeof(NativeApi.mtmd_context_params), "device", 8, 4)]
        [InlineData(typeof(NativeApi.mtmd_context_params), "print_timings", 16, 8)]
        [InlineData(typeof(NativeApi.mtmd_context_params), "n_threads", 20, 12)]
        [InlineData(typeof(NativeApi.mtmd_context_params), "batch_max_tokens", 72, 48)]
        [InlineData(typeof(NativeApi.mtmd_helper_video_init_params), "timestamp_interval_ms", 16, 8)]
        [InlineData(typeof(NativeApi.mtmd_helper_video_init_params), "no_soundtrack", 24, 16)]
        [InlineData(typeof(LLamaSamplerINative), "BackendReset", 80, 40)]
        [InlineData(typeof(LLamaSamplerINative), "CopyState", 88, 44)]
        public void ParameterOffsetsMatchNative(Type type, string field, int offset64, int offset32)
        {
            Assert.Equal(IntPtr.Size == 8 ? offset64 : offset32, Marshal.OffsetOf(type, field).ToInt32());
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
