using LLama.Extensions;

namespace LLama.Unittest;

public class IReadOnlyListExtensionsTests
{
    [Fact]
    public void IndexOfItem()
    {
        var items = (IReadOnlyList<int>)new List<int> { 1, 2, 3, 4, };

        Assert.Equal(2, items.IndexOf(3));
    }

    [Fact]
    public void IndexOfItemNotFound()
    {
        var items = (IReadOnlyList<int>)new List<int> { 1, 2, 3, 4, };

        Assert.Null(items.IndexOf(42));
    }
}