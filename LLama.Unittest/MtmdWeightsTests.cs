using System;
using System.IO;
using LLama.Common;
using LLama.Native;
using Xunit;

namespace LLama.Unittest
{
    // Test the same things as llama model + image embedings
    //
    public sealed class MtmdWeightTests
        : IDisposable
    {
        private readonly LLamaWeights _llamaWeights;
        private readonly SafeMtmdWeights _safeMtmdWeights;
        private readonly LLamaContext _context;
        private readonly MtmdContextParams _mtmdParams;
        private readonly string _mediaMarker;

        public MtmdWeightTests()
        {
            var @params = new ModelParams(Constants.MtmdModelPath)
            {
                // Mtmd models requires big context
                ContextSize = 1024 * 32,
                GpuLayerCount = Constants.CIGpuLayerCount,
            };
            _llamaWeights = LLamaWeights.LoadFromFile(@params);

            _mtmdParams = MtmdContextParams.Default();
            _mtmdParams.NThreads = Constants.CIGpuLayerCount;
            _mtmdParams.UseGpu = false; // keep tests portable across environments without GPU

            _mediaMarker = _mtmdParams.MediaMarker ?? throw new InvalidOperationException("MTMD media marker unavailable.");

            _safeMtmdWeights = SafeMtmdWeights.LoadFromFile(Constants.MtmdMmpPath, _llamaWeights, _mtmdParams);
            _context = _llamaWeights.CreateContext(@params);
        }

        public void Dispose()
        {
            _context.Dispose();
            _safeMtmdWeights.Dispose();
            _llamaWeights.Dispose();
        }

        private SafeMtmdInputChunks TokenizeWithEmbed(Func<SafeMtmdEmbed> loadEmbed)
        {
            _safeMtmdWeights.ClearMedia();

            var embed = loadEmbed();
            Assert.NotNull(embed);

            using (embed)
            {
                Assert.True(embed.Nx > 0);
                Assert.True(embed.Ny > 0);
                Assert.False(embed.IsAudio);
                Assert.True(embed.GetDataSpan().Length > 0);

                var status = _safeMtmdWeights.Tokenize(_mediaMarker, addSpecial: true, parseSpecial: true, out var chunks);
                Assert.Equal(0, status);
                Assert.NotNull(chunks);

                return chunks!;
            }
        }

        private void AssertChunksEvaluate(SafeMtmdInputChunks chunks)
        {
            long nPast = 0;
            var eval = _safeMtmdWeights.EvaluateChunks(chunks, _context.NativeHandle, ref nPast, seqId: 0, nBatch: checked((int)_context.BatchSize), logitsLast: true);
            Assert.Equal(0, eval);
            Assert.True(nPast > 0);
        }

        [Fact,Trait("Category", "NoCI")]
        public void EmbedImageAsFileName()
        {
            using var chunks = TokenizeWithEmbed(() => _safeMtmdWeights.LoadMedia(Constants.MtmdImage));
            AssertChunksEvaluate(chunks);
        }

        [Fact,Trait("Category", "NoCI")]
        public void EmbedImageAsBinary()
        {
            var imageBytes = File.ReadAllBytes(Constants.MtmdImage);
            using var chunks = TokenizeWithEmbed(() => _safeMtmdWeights.LoadMedia(imageBytes));
            AssertChunksEvaluate(chunks);
        }

        [Fact,Trait("Category", "NoCI")]
        public void TokenizeProvidesChunkMetadata()
        {
            using var chunks = TokenizeWithEmbed(() => _safeMtmdWeights.LoadMedia(Constants.MtmdImage));

            Assert.True(chunks.Size > 0);

            ulong totalTokens = 0;
            long totalPositions = 0;
            var imageChunks = 0;

            foreach (var chunk in chunks.Enumerate())
            {
                totalTokens += chunk.NTokens;
                totalPositions += chunk.NPos;

                if (chunk.Type == SafeMtmdInputChunk.SafeMtmdInputChunkType.Image)
                {
                    imageChunks++;

                    var copy = chunk.Copy();
                    try
                    {
                        Assert.NotNull(copy);
                        if (copy != null)
                        {
                            Assert.Equal(chunk.NTokens, copy.NTokens);
                            Assert.Equal(chunk.NPos, copy.NPos);
                        }
                    }
                    finally
                    {
                        copy?.Dispose();
                    }
                }
            }

            Assert.True(imageChunks > 0);
            Assert.True(totalTokens > 0);
            Assert.Equal(totalTokens, _safeMtmdWeights.CountTokens(chunks));
            Assert.Equal(totalPositions, _safeMtmdWeights.CountPositions(chunks));
            Assert.True(_safeMtmdWeights.SupportsVision);
            Assert.False(_safeMtmdWeights.SupportsAudio);

            var audioBitrate = _safeMtmdWeights.AudioBitrate;
            Assert.True(audioBitrate <= 0);
        }
    }
}
