using System.Runtime.InteropServices;
using LLama.Native;

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
        public void ContextParamsSizeMatchesNative()
        {
            var pointerSize = IntPtr.Size;
            var fields = new List<(int size, int align)>
            {
                (sizeof(uint), 4), // n_ctx
                (sizeof(uint), 4), // n_batch
                (sizeof(uint), 4), // n_ubatch
                (sizeof(uint), 4), // n_seq_max
                (sizeof(int), 4),  // n_threads
                (sizeof(int), 4),  // n_threads_batch
                (sizeof(int), 4),  // rope_scaling_type
                (sizeof(int), 4),  // pooling_type
                (sizeof(int), 4),  // attention_type
                (sizeof(int), 4),  // flash_attn_type
                (sizeof(float), 4), // rope_freq_base
                (sizeof(float), 4), // rope_freq_scale
                (sizeof(float), 4), // yarn_ext_factor
                (sizeof(float), 4), // yarn_attn_factor
                (sizeof(float), 4), // yarn_beta_fast
                (sizeof(float), 4), // yarn_beta_slow
                (sizeof(uint), 4),  // yarn_orig_ctx
                (sizeof(float), 4), // defrag_thold
                (pointerSize, pointerSize), // cb_eval
                (pointerSize, pointerSize), // cb_eval_user_data
                (sizeof(int), 4),  // type_k
                (sizeof(int), 4),  // type_v
                (pointerSize, pointerSize), // abort_callback
                (pointerSize, pointerSize), // abort_callback_user_data
                (sizeof(sbyte), 1), // embeddings
                (sizeof(sbyte), 1), // offload_kqv
                (sizeof(sbyte), 1), // no_perf
                (sizeof(sbyte), 1), // op_offload
                (sizeof(sbyte), 1), // swa_full
                (sizeof(sbyte), 1), // kv_unified
                (pointerSize, pointerSize), // samplers
                (pointerSize, pointerSize), // n_samplers
            };

            var expectedSize = ComputeSize(fields);
            Assert.Equal(expectedSize, Marshal.SizeOf<LLamaContextParams>());
        }

        [Fact]
        public void ModelParamsBoolBlockMatchesNative()
        {
            var pointerSize = IntPtr.Size;
            var kvOffset = Marshal.OffsetOf<LLamaModelParams>("kv_overrides").ToInt32();
            var vocabOffset = Marshal.OffsetOf<LLamaModelParams>("_vocab_only").ToInt32();

            Assert.Equal(kvOffset + pointerSize, vocabOffset);
            Assert.Equal(vocabOffset + 1, Marshal.OffsetOf<LLamaModelParams>("_use_mmap").ToInt32());
            Assert.Equal(vocabOffset + 2, Marshal.OffsetOf<LLamaModelParams>("_use_direct_io").ToInt32());
            Assert.Equal(vocabOffset + 3, Marshal.OffsetOf<LLamaModelParams>("_use_mlock").ToInt32());
            Assert.Equal(vocabOffset + 4, Marshal.OffsetOf<LLamaModelParams>("_check_tensors").ToInt32());
            Assert.Equal(vocabOffset + 5, Marshal.OffsetOf<LLamaModelParams>("_use_extra_bufts").ToInt32());
            Assert.Equal(vocabOffset + 6, Marshal.OffsetOf<LLamaModelParams>("_no_host").ToInt32());
            Assert.Equal(vocabOffset + 7, Marshal.OffsetOf<LLamaModelParams>("_no_alloc").ToInt32());
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
