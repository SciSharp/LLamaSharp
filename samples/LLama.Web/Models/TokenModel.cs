namespace LLama.Web.Models;

public class TokenModel
{
    public TokenModel(string id, string content = null, TokenType tokenType = TokenType.Content)
    {
        Id = id;
        Content = content;
        TokenType = tokenType;
    }

    public string Id { get; set; }
    public string Content { get; set; }
    public TokenType TokenType { get; set; }
}

public enum TokenType
{
    Begin = 0,
    Content = 2,
    End = 4,
    Cancel = 10
}
