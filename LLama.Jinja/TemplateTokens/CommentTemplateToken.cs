namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class CommentTemplateToken : TemplateToken
{
    public readonly string Comment;

    public CommentTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, string comment)
        : base(TemplateType.Comment, location, preSpace, postSpace)
    {
        Comment = comment;
    }

    public override string ToString()
    {
        return $"Comment: {Comment}";
    }
}

