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

        public LLamaContextWithCustomLoggerTests()
        {
            var @params = new ModelParams(Constants.GenerativeModelPath)
            {
                ContextSize = 128,
                GpuLayerCount = Constants.CIGpuLayerCount,
            };

            // This unit test used to fail when loading the weights with such a naive logger set.
            //
            // See https://github.com/SciSharp/LLamaSharp/issues/995
            //
            // So the unit test here doesn't check that the logger is actually used
            // but at least that setting one doesn't crash the weights load.
            NativeLogConfig.llama_log_set(new CustomLogger());

            _weights = LLamaWeights.LoadFromFile(@params);
            _context = _weights.CreateContext(@params);
        }

        public void Dispose()
        {
            _weights.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void CheckProperties()
        {
            Assert.Equal(128u, _context.ContextSize);
            Assert.Equal(2048, _context.EmbeddingSize);
            Assert.Equal(128256, _context.VocabCount);
        }
    }
}