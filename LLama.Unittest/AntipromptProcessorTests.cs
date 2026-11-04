using System.Globalization;

namespace LLama.Unittest;

public class AntipromptProcessorTests
{
    [Fact]
    public void MatchesAntipromptRegardlessOfCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            // In Hungarian "dz" is a single letter, so a culture sensitive comparison
            // says "adz" does not end with "z".
            CultureInfo.CurrentCulture = new CultureInfo("hu-HU");

            var processor = new AntipromptProcessor(["z"]);

            Assert.True(processor.Add("adz"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void DoesNotMatchWhenTextDoesNotEndWithAntiprompt()
    {
        var processor = new AntipromptProcessor(["User:"]);

        Assert.False(processor.Add("User: hello"));
    }
}
