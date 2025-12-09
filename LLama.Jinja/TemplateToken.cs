namespace LLamaSharp.Jinja;

internal abstract class TemplateToken
{
    public readonly Location Location;
    public readonly SpaceHandling PreSpace;
    public readonly SpaceHandling PostSpace;
    public readonly TemplateType Type;

    public enum TemplateType
    {
        Text,
        Expression,
        If,
        Else,
        Elif,
        EndIf,
        For,
        EndFor,
        Generation,
        EndGeneration,
        Set,
        EndSet,
        Comment,
        Macro,
        EndMacro,
        Filter,
        EndFilter,
        Break,
        Continue,
        Call,
        EndCall
    };

    protected TemplateToken(TemplateType type, Location location, SpaceHandling preSpace, SpaceHandling postSpace)
    {
        ArgumentNullException.ThrowIfNull(location);
        Type = type;
        Location = location;
        PreSpace = preSpace;
        PostSpace = postSpace;
    }

    public static string TypeToString(TemplateType type)
    {
        return type switch
        {
            TemplateType.Text => "text",
            TemplateType.Expression => "expression",
            TemplateType.If => "if",
            TemplateType.Else => "else",
            TemplateType.Elif => "elif",
            TemplateType.EndIf => "endif",
            TemplateType.For => "for",
            TemplateType.EndFor => "endfor",
            TemplateType.Set => "set",
            TemplateType.EndSet => "endset",
            TemplateType.Comment => "comment",
            TemplateType.Macro => "macro",
            TemplateType.EndMacro => "endmacro",
            TemplateType.Filter => "filter",
            TemplateType.EndFilter => "endfilter",
            TemplateType.Generation => "generation",
            TemplateType.EndGeneration => "endgeneration",
            TemplateType.Break => "break",
            TemplateType.Continue => "continue",
            TemplateType.Call => "call",
            TemplateType.EndCall => "endcall",
            _ => "Unknown",
        };
    }
}

