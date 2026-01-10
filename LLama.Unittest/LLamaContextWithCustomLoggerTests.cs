using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Logging;

namespace LLama.Unittest
{
    public sealed class LLamaContextWithCustomLoggerTests
        : IDisposable
    {
        private sealed class CustomLogger : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception, string> formatter)
            {
            }

            public bool IsEnabled(LogLevel logLevel) => true;
        }

        private readonly LLamaWeights _weights;
        private readonly LLamaContext _context;
        private readonly ModelParams _params;

        public LLamaContextWithCustomLoggerTests()
        {
            _params = new ModelParams(Constants.GenerativeModelPath2)
            {
                ContextSize = 512,
                GpuLayerCount = Constants.CIGpuLayerCount,
            };

            // This unit test used to fail when loading the weights with such a naive logger set.
            //
            // See https://github.com/SciSharp/LLamaSharp/issues/995
            //
            // So the unit test here doesn't check that the logger is actually used
            // but at least that setting one doesn't crash the weights load.
            NativeLogConfig.llama_log_set(new CustomLogger());

            _weights = LLamaWeights.LoadFromFile(_params);
            _context = _weights.CreateContext(_params);
        }

        public void Dispose()
        {
            _weights.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void CheckProperties()
        {
            Assert.Equal(_params.ContextSize ?? 0, _context.ContextSize);
            Assert.Equal(960, _context.EmbeddingSize);
            Assert.Equal(49152, _context.Vocab.Count);
        }
    }
}
