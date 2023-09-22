using LLama.Extensions;

namespace LLama.Unittest;

public class IEnumerableExtensionsTests
{
    [Fact]
    public void TakeLastEmpty()
    {
        var arr = Array.Empty<int>();

        var last = IEnumerableExtensions.TakeLastImpl(arr, 5).ToList();

        Assert.Empty(last);
    }

    [Fact]
    public void TakeLastAll()
    {
        var arr = new[] { 1, 2, 3, 4, 5 };

        var last = IEnumerableExtensions.TakeLastImpl(arr, 5).ToList();

        Assert.True(last.SequenceEqual(arr));
    }

    [Fact]
    public void TakeLast()
    {
        var arr = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var last = IEnumerableExtensions.TakeLastImpl(arr, 5).ToList();

        Assert.True(last.SequenceEqual(arr[5..]));
    }
}