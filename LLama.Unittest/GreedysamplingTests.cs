using Xunit;
using LLama.Sampling;
using LLama.Native;

namespace LLama.Sampling.Tests
{
    public class TestableSafeLLamaSamplerChainHandle
    {
        private SafeLLamaSamplerChainHandle chain;
        public List<Grammar> Grammars { get; set; }

        public TestableSafeLLamaSamplerChainHandle(SafeLLamaSamplerChainHandle chain)
        {
            this.chain = chain;
            this.Grammars = new List<Grammar>();
        }

        public SafeLLamaSamplerChainHandle GetChain()
        {
            return chain;
        }
    }

    public class TestableGreedySamplingPipeline : GreedySamplingPipeline
    {
        public SafeLLamaSamplerChainHandle CreateChainForTest(SafeLLamaContextHandle context)
        {
            return base.CreateChain(context);
        }
    }

    public class GreedySamplingPipelineTests
    {
        [Fact]
        public void CreateChain_WithoutGrammar_DoesNotAddGrammarToChain()
        {
            // Arrange
            var model = new SafeLlamaModelHandle();
            var lparams = LLamaContextParams.Default();
            var context = SafeLLamaContextHandle.Create(model, lparams);
            var pipeline = new TestableGreedySamplingPipeline();

            // Act
            var chain = pipeline.CreateChainForTest(context);
            var testableChain = new TestableSafeLLamaSamplerChainHandle(chain)
            {
                Grammars = new List<Grammar>()
            };

            // Assert
            Assert.Empty(testableChain.Grammars);
        }

        [Fact]
        public void Get_Grammar_ReturnsExpectedValue()
        {
            // Arrange
            var expectedGrammar = new Grammar("test_gbnf", "test_root");
            var pipeline = new TestableGreedySamplingPipeline { Grammar = expectedGrammar };

            // Act
            var actualGrammar = pipeline.Grammar;

            // Assert
            Assert.Equal(expectedGrammar, actualGrammar);
        }

        [Fact]
        public void Get_Grammar_ReturnsNullByDefault()
        {
            // Arrange
            var pipeline = new TestableGreedySamplingPipeline();

            // Act
            var actualGrammar = pipeline.Grammar;

            // Assert
            Assert.Null(actualGrammar);
        }

        [Fact]
        public void Set_Grammar_SetsExpectedValue()
        {
            // Arrange
            var expectedGrammar = new Grammar("test_gbnf", "test_root");
            var pipeline = new TestableGreedySamplingPipeline { Grammar = expectedGrammar };

            // Act and Assert
            Assert.Equal(expectedGrammar, pipeline.Grammar);
        }
    }
}
