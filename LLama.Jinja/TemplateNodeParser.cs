using System.Text.RegularExpressions;

namespace LLamaSharp.Jinja;

using TemplateNodes;
using TemplateTokens;

internal sealed partial class TemplateNodeParser
{
    private readonly string _templateString;
    private readonly Options _options;
    private readonly IReadOnlyList<TemplateToken> _tokens;
    private int _it;

    public TemplateNodeParser(string templateString, Options options, IReadOnlyList<TemplateToken> tokens)
    {
        ArgumentNullException.ThrowIfNull(templateString);
        _templateString = templateString;
        _it = 0;
        _options = options;
        _tokens = tokens;
    }

    private JinjaException Unexpected(TemplateToken token)
    {
        return new JinjaException($"Unexpected {TemplateToken.TypeToString(token.Type)}{LocationExtensions.ToString(_templateString, token.Location.Position)}");
    }

    private JinjaException Unterminated(TemplateToken token)
    {
        return new JinjaException($"Unterminated {TemplateToken.TypeToString(token.Type)}{LocationExtensions.ToString(_templateString, token.Location.Position)}");
    }

    public TemplateNode ParseTemplate(bool fully = false)
    {
        var children = new List<TemplateNode>();
        var end = _tokens.Count;

        while (_it < end)
        {
            var start = _it;
            var token = _tokens[_it++];

            if (token is IfTemplateToken ifToken)
            {
                var cascade = new List<(Expression? Expression, TemplateNode TemplateNode)> { (ifToken.Condition, ParseTemplate()) };

                while (_it < end && _tokens[_it].Type == TemplateToken.TemplateType.Elif)
                {
                    var elifToken = (ElIfTemplateToken)_tokens[_it++];
                    cascade.Add((elifToken.Condition, ParseTemplate()));
                }

                if (_it < end && _tokens[_it].Type == TemplateToken.TemplateType.Else)
                {
                    _it++;
                    cascade.Add((null, ParseTemplate()));
                }
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndIf)
                    throw Unterminated(_tokens[start]);
                children.Add(new IfNode(token.Location, cascade));
            }
            else if (token is ForTemplateToken forToken)
            {
                var body = ParseTemplate();
                TemplateNode? elseBody = null;
                if (_it < end && _tokens[_it].Type == TemplateToken.TemplateType.Else)
                {
                    _it++;
                    elseBody = ParseTemplate();
                }
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndFor)
                    throw Unterminated(_tokens[start]);
                children.Add(new ForNode(token.Location,
                    forToken.VariableNames,
                    forToken.Iterable,
                    forToken.Condition,
                    body,
                    forToken.Recursive,
                    elseBody));
            }
            else if (token is GenerationTemplateToken)
            {
                var body = ParseTemplate();
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndGeneration)
                    throw Unterminated(_tokens[start]);
                // Treat as a no-op, as our scope is templates for inference, not training (`{% generation %}` wraps generated tokens for masking).
                children.Add(body);
            }
            else if (token is TextTemplateToken textToken)
            {
                var preSpace = (_it - 1) > 0 ? _tokens[_it - 2].PostSpace : SpaceHandling.Keep;
                var postSpace = _it < end ? _tokens[_it].PreSpace : SpaceHandling.Keep;
                var text = textToken.Text;

                if (postSpace == SpaceHandling.Strip)
                    text = TrailingSpaceRegex().Replace(text, "");
                else if (_options.LStripBlocks && _it < end)
                {
                    var i = text.Length;
                    while (i > 0 && (text[i - 1] == ' ' || text[i - 1] == '\t'))
                        --i;
                    if ((i == 0 && (_it - 1) == 0) || (i > 0 && text[i - 1] == '\n'))
                        text = text[..i];
                }

                if (preSpace == SpaceHandling.Strip)
                    text = LeadingSpaceRegex().Replace(text, "");
                else if (_options.TrimBlocks && (_it - 1) > 0 && _tokens[_it - 2] is not ExpressionTemplateToken)
                    if (!string.IsNullOrEmpty(text) && text[0] == '\n')
                        text = text[1..];

                if (_it == end && !_options.KeepTrailingNewline)
                {
                    var i = text.Length;
                    if (i > 0 && text[i - 1] == '\n')
                    {
                        --i;
                        if (i > 0 && text[i - 1] == '\r')
                            --i;
                        text = text[..i];
                    }
                }
                children.Add(new TextNode(token.Location, text));
            }
            else if (token is ExpressionTemplateToken exprToken)
            {
                children.Add(new ExpressionNode(token.Location, exprToken.Expression));
            }
            else if (token is SetTemplateToken setToken)
            {
                if (setToken.Value != null)
                    children.Add(new SetNode(token.Location, setToken.Namespace, setToken.VariableNames, setToken.Value));
                else
                {
                    var valueTemplate = ParseTemplate();
                    if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndSet)
                        throw Unterminated(_tokens[start]);
                    if (!string.IsNullOrEmpty(setToken.Namespace))
                        throw new JinjaException("Namespaced set not supported in set with template value");
                    if (setToken.VariableNames.Count != 1)
                        throw new JinjaException("Structural assignment not supported in set with template value");
                    var name = setToken.VariableNames.Single();
                    children.Add(new SetTemplateNode(token.Location, name, valueTemplate));
                }
            }
            else if (token is MacroTemplateToken macroToken)
            {
                var body = ParseTemplate();
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndMacro)
                    throw Unterminated(_tokens[start]);
                children.Add(new MacroNode(token.Location, macroToken.Name, macroToken.Parameters.ToList(), body));
            }
            else if (token is CallTemplateToken callToken)
            {
                var body = ParseTemplate();
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndCall)
                    throw Unterminated(_tokens[start]);
                children.Add(new CallNode(token.Location, callToken.Callee, body));
            }
            else if (token is FilterTemplateToken filterToken)
            {
                var body = ParseTemplate();
                if (_it == end || _tokens[_it++].Type != TemplateToken.TemplateType.EndFilter)
                    throw Unterminated(_tokens[start]);
                children.Add(new FilterNode(token.Location, filterToken.Filter, body));
            }
            else if (token is CommentTemplateToken)
            {
                // Ignore comments
            }
            else if (token is LoopControlTemplateToken ctrlToken)
                children.Add(new LoopControlNode(token.Location, ctrlToken.ControlType));
            else if (
                token is EndForTemplateToken ||
                token is EndSetTemplateToken ||
                token is EndMacroTemplateToken ||
                token is EndCallTemplateToken ||
                token is EndFilterTemplateToken ||
                token is EndIfTemplateToken ||
                token is ElseTemplateToken ||
                token is EndGenerationTemplateToken ||
                token is ElIfTemplateToken)
            {
                _it--; // unconsume
                break;
            }
            else
                throw Unexpected(_tokens[_it - 1]);
        }

        if (fully && _it != end)
            throw Unexpected(_tokens[_it]);

        if (children.Count == 0)
            return new TextNode(new Location { Source = _templateString, Position = 0 }, string.Empty);
        else if (children.Count == 1)
            return children[0];
        else
            return new SequenceNode(children[0].Location, children);
    }

    [GeneratedRegex(@"\s+$")]
    private static partial Regex TrailingSpaceRegex();
    [GeneratedRegex(@"^\s+")]
    private static partial Regex LeadingSpaceRegex();
}

