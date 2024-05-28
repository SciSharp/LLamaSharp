using LLama.Common;

namespace LLama.Unittest;

public class FixedSizeQueueTests
{
    [Fact]
    public void Create()
    {
        var q = new FixedSizeQueue<int>(7);

        Assert.Equal(7, q.Capacity);
        Assert.Empty(q);
    }

    [Fact]
    public void CreateFromItems()
    {
        var q = new FixedSizeQueue<int>(7, new [] { 1, 2, 3 });

        Assert.Equal(7, q.Capacity);
        Assert.Equal(3, q.Count);
        Assert.True(q.ToArray().SequenceEqual(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void Indexing()
    {
        var q = new FixedSizeQueue<int>(7, new[] { 1, 2, 3 });

        Assert.Equal(1, q[0]);
        Assert.Equal(2, q[1]);
        Assert.Equal(3, q[2]);

        Assert.Throws<ArgumentOutOfRangeException>(() => q[3]);
    }

    [Fact]
    public void CreateFromFullItems()
    {
        var q = new FixedSizeQueue<int>(3, new[] { 1, 2, 3 });

        Assert.Equal(3, q.Capacity);
        Assert.Equal(3, q.Count);
        Assert.True(q.ToArray().SequenceEqual(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void CreateFromTooManyItems()
    {
        Assert.Throws<ArgumentException>(() => new FixedSizeQueue<int>(2, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void CreateFromTooManyItemsNonCountable()
    {
        Assert.Throws<ArgumentException>(() => new FixedSizeQueue<int>(2, Items()));
        return;

        static IEnumerable<int> Items()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    [Fact]
    public void Enqueue()
    {
        var q = new FixedSizeQueue<int>(7, new[] { 1, 2, 3 });

        q.Enqueue(4);
        q.Enqueue(5);

        Assert.Equal(7, q.Capacity);
        Assert.Equal(5, q.Count);
        Assert.True(q.ToArray().SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
    }

    [Fact]
    public void EnqueueOverflow()
    {
        var q = new FixedSizeQueue<int>(5, new[] { 1, 2, 3 });

        q.Enqueue(4);
        q.Enqueue(5);
        q.Enqueue(6);
        q.Enqueue(7);

        Assert.Equal(5, q.Capacity);
        Assert.Equal(5, q.Count);
        Assert.True(q.ToArray().SequenceEqual(new[] { 3, 4, 5, 6, 7 }));
    }
}