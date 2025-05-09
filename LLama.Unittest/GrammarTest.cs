using Xunit;

namespace LLama.Sampling.Tests
{
    public class GrammarTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var gbnf = "test_gbnf";
            var root = "test_root";

            // Act
            var grammar = new Grammar(gbnf, root);

            // Assert
            Assert.Equal(gbnf, grammar.Gbnf);
            Assert.Equal(root, grammar.Root);
        }

        [Fact]
        public void ToString_ReturnsExpectedString()
        {
            // Arrange
            var gbnf = "test_gbnf";
            var root = "test_root";
            var grammar = new Grammar(gbnf, root);

            // Act
            var toString = grammar.ToString();

            // Assert
            Assert.Equal($"Grammar {{ Gbnf = {gbnf}, Root = {root} }}", toString);
        }

        [Fact]
        public void Equality_ChecksPropertiesCorrectly()
        {
            // Arrange
            var gbnf = "test_gbnf";
            var root = "test_root";
            var grammar1 = new Grammar(gbnf, root);
            var grammar2 = new Grammar(gbnf, root);

            // Act and Assert
            Assert.Equal(grammar1, grammar2);
        }

        [Fact]
        public void Inequality_ChecksPropertiesCorrectly()
        {
            // Arrange
            var gbnf = "test_gbnf";
            var root = "test_root";
            var grammar1 = new Grammar(gbnf, root);
            var grammar2 = new Grammar("different_gbnf", root);

            // Act and Assert
            Assert.NotEqual(grammar1, grammar2);
        }
    }
}
