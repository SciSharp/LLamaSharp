using System.Text;

namespace LLama.Unittest;

public sealed class TemplateRoleCacheTests
{
    [Fact]
    public void RoleBytesAreSetWhenRoleCacheIsFull()
    {
        var templater = new LLamaTemplate("chatml");

        // Use more distinct roles than the role cache can hold
        for (var i = 0; i < 200; i++)
            templater.Add($"role{i}", "hello");

        for (var i = 0; i < templater.Count; i++)
        {
            var roleBytes = templater[i].RoleBytes.Span;
            Assert.Equal($"role{i}\0", Encoding.UTF8.GetString(roleBytes));
        }
    }
}
