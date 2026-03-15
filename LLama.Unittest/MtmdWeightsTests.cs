using LLama.Common;
using LLama.Native;

namespace LLama.Unittest
{
    // Test the same things as llama model + image embedings
    //
    [Collection("MtmdNoCi")]
    public sealed class MtmdWeightTests
    {
        private readonly MtmdNoCiFixture _fixture;
        private readonly string _mediaMarker;

        public MtmdWeightTests(MtmdNoCiFixture fixture)
        {
            _fixture = fixture;
            _mediaMarker = _fixture.MediaMarker;
        }

        private SafeMtmdInputChunks TokenizeWithEmbed(Func<SafeMtmdEmbed> loadEmbed)
        {
            _fixture.Mtmd.ClearMedia();

            var embed = loadEmbed();
            Assert.NotNull(embed);

            using (embed)
            {
                Assert.True(embed.Nx > 0);
                Assert.True(embed.Ny > 0);
                Assert.False(embed.IsAudio);

                Assert.True(embed.ByteCount > 0);
                using var mem = embed.GetData();
                Assert.True(mem.Data.Length > 0);

                var status = _fixture.Mtmd.Tokenize(_mediaMarker, addSpecial: true, parseSpecial: true, out var chunks);
                Assert.Equal(0, status);
                Assert.NotNull(chunks);

                return chunks!;
            }
        }

        private void AssertChunksEvaluate(SafeMtmdInputChunks chunks)
        {
            using var context = _fixture.CreateContext();
            int nPast = 0;
            var eval = _fixture.Mtmd.EvaluateChunks(chunks, context.NativeHandle, ref nPast, seqId: 0, nBatch: checked((int)context.BatchSize), logitsLast: true);
            Assert.Equal(0, eval);
            Assert.True(nPast > 0);
        }

        [SkippableFact, Trait("Category", "NoCI")]
        public void BasicPropertyChecks()
        {
            Skip.IfNot(_fixture.IsEnabled, MtmdNoCiFixture.SkipReason);

            Assert.False(_fixture.Mtmd.SupportsAudio);
            Assert.True(_fixture.Mtmd.SupportsVision);
            Assert.False(_fixture.Mtmd.UsesMRope);
            Assert.True(_fixture.Mtmd.UsesNonCausalAttention);
            Assert.Equal(-1, _fixture.Mtmd.AudioBitrate);
        }

        [SkippableFact, Trait("Category", "NoCI")]
        public void EmbedImageAsFileName()
        {
            Skip.IfNot(_fixture.IsEnabled, MtmdNoCiFixture.SkipReason);

            using var chunks = TokenizeWithEmbed(() => _fixture.Mtmd.LoadMedia(Constants.MtmdImage));
            AssertChunksEvaluate(chunks);
        }

        [SkippableFact, Trait("Category", "NoCI")]
        public void EmbedImageAsBinary()
        {
            Skip.IfNot(_fixture.IsEnabled, MtmdNoCiFixture.SkipReason);

            var imageBytes = File.ReadAllBytes(Constants.MtmdImage);
            using var chunks = TokenizeWithEmbed(() => _fixture.Mtmd.LoadMedia(imageBytes));
            AssertChunksEvaluate(chunks);
        }

        [SkippableFact, Trait("Category", "NoCI")]
        public void TokenizeProvidesChunkMetadata()
        {
            Skip.IfNot(_fixture.IsEnabled, MtmdNoCiFixture.SkipReason);

            using var chunks = TokenizeWithEmbed(() => _fixture.Mtmd.LoadMedia(Constants.MtmdImage));

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
            Assert.Equal(totalTokens, chunks.CountTokens());
            Assert.Equal(totalPositions, chunks.CountPositions());
            Assert.True(_fixture.Mtmd.SupportsVision);
            Assert.False(_fixture.Mtmd.SupportsAudio);

            var audioBitrate = _fixture.Mtmd.AudioBitrate;
            Assert.True(audioBitrate <= 0);
        }
    }
}
