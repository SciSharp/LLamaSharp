using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LLamaSharp.Jinja;

using Expressions;
using TemplateTokens;

internal sealed partial class Tokenizer
{
    private readonly string _templateString;
    private readonly int _start;
    private readonly int _end;
    private int _it;

    public Tokenizer(string templateString)
    {
        ArgumentNullException.ThrowIfNull(templateString);
        _templateString = templateString;
        _start = _it = 0;
        _end = templateString.Length;
    }

    private bool ConsumeSpaces(SpaceHandling spaceHandling = SpaceHandling.Strip)
    {
        if (spaceHandling == SpaceHandling.Strip)
            while (_it < _end && char.IsWhiteSpace(_templateString[_it]))
                ++_it;
        return true;
    }

    private string? ParseString()
    {
        string? doParse(char quote)
        {
            if (_it == _end || _templateString[_it] != quote)
                return null;
            var result = new StringBuilder();
            var escape = false;
            for (++_it; _it < _end; ++_it)
            {
                var ch = _templateString[_it];
                if (escape)
                {
                    escape = false;
                    switch (ch)
                    {
                        case 'n':
                            result.Append('\n');
                            break;
                        case 'r':
                            result.Append('\r');
                            break;
                        case 't':
                            result.Append('\t');
                            break;
                        case 'b':
                            result.Append('\b');
                            break;
                        case 'f':
                            result.Append('\f');
                            break;
                        case '\\':
                            result.Append('\\');
                            break;
                        default:
                            if (ch == quote)
                                result.Append(quote);
                            else
                                result.Append(ch);
                            break;
                    }
                }
                else if (ch == '\\')
                    escape = true;
                else if (ch == quote)
                {
                    ++_it;
                    return result.ToString();
                }
                else
                    result.Append(_templateString[_it]);
            }
            return null;
        }
        ;
        ConsumeSpaces();
        if (_it == _end)
            return null;
        if (_templateString[_it] == '"' || _templateString[_it] == '\'')
            return doParse(_templateString[_it]);
        return null;
    }

    private Value? ParseNumber()
    {
        var before = _it;
        ConsumeSpaces();
        var start = _it;
        var hasDecimal = false;
        var hasExponent = false;

        if (_it < _end && (_templateString[_it] == '+' || _templateString[_it] == '-'))
            ++_it;

        while (_it < _end)
        {
            if (char.IsDigit(_templateString[_it]))
                ++_it;
            else if (_templateString[_it] == '.')
            {
                if (hasDecimal)
                    throw new JinjaException("Multiple decimal points");
                hasDecimal = true;
                ++_it;
            }
            else if (_it != start && (_templateString[_it] == 'e' || _templateString[_it] == 'E'))
            {
                if (hasExponent)
                    throw new JinjaException("Multiple exponents");
                hasExponent = true;
                ++_it;
            }
            else
                break;
        }
        if (start == _it)
        {
            _it = before;
            return null; // No valid character found
        }

        var str = _templateString[start.._it];
        if (long.TryParse(str, CultureInfo.InvariantCulture, out var intValue))
            return new Value(intValue);
        else if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
            return new Value(doubleValue);
        else
            throw new JinjaException($"Failed to parse number: {str}");
    }

    /// <summary>
    /// Parse integer, float, bool or string constant
    /// </summary>
    /// <returns></returns>
    private Value? ParseConstant()
    {
        var start = _it;
        ConsumeSpaces();
        if (_it == _end)
            return null;
        if (_templateString[_it] == '"' || _templateString[_it] == '\'')
        {
            var str = ParseString();
            if (str is not null)
                return new Value(str);
        }
        var token = ConsumeToken(ConstantTokenRegex());
        if (!string.IsNullOrEmpty(token))
        {
            return token switch
            {
                "true" or "True" => new Value(true),
                "false" or "False" => new Value(false),
                "None" => Value.Null,
                _ => throw new JinjaException($"Unknown constant token: {token}"),
            };
        }

        var number = ParseNumber();
        if (number is not null)
            return number;
        _it = start;
        return null;
    }

    private bool PeekSymbols(IEnumerable<string> symbols)
    {
        foreach (var symbol in symbols)
        {
            var len = symbol.Length;
            if (_it + len <= _end && _templateString.Substring(_it, len) == symbol)
                return true;
        }
        return false;
    }

    private List<string> ConsumeTokenGroups(Regex regex, SpaceHandling spaceHandling = SpaceHandling.Strip)
    {
        var start = _it;
        ConsumeSpaces(spaceHandling);
        var match = regex.Match(_templateString, _it, _end - _it);
        if (match.Success && match.Index == _it)
        {
            _it += match.Length;
            var groups = new List<string>();
            for (var i = 1; i < match.Groups.Count; ++i)
                groups.Add(match.Groups[i].Value);
            return groups;
        }
        _it = start;
        return [];
    }

    private string ConsumeToken(Regex regex, SpaceHandling spaceHandling = SpaceHandling.Strip)
    {
        var start = _it;
        ConsumeSpaces(spaceHandling);
        var match = regex.Match(_templateString, _it, _end - _it);
        if (match.Success && match.Index == _it)
        {
            _it += match.Length;
            return match.Value;
        }
        _it = start;
        return "";
    }

    private string ConsumeToken(string token, SpaceHandling spaceHandling = SpaceHandling.Strip)
    {
        var start = _it;
        ConsumeSpaces(spaceHandling);
        var len = token.Length;
        if (_it + len <= _end && _templateString.Substring(_it, len) == token)
        {
            _it += len;
            return token;
        }
        _it = start;
        return "";
    }

    private Expression? ParseExpression(bool allowIfExpression = true)
    {
        var left = ParseLogicalOr();
        if (_it == _end)
            return left;
        if (!allowIfExpression)
            return left;
        var token = ConsumeToken(IfTokenRegex());
        if (string.IsNullOrEmpty(token))
            return left;

        var location = Location;
        var (condition, elseExpression) = ParseIfExpression();
        return new IfExpr(location, condition, left!, elseExpression);
    }

    private Location Location => new() { Source = _templateString, Position = _it - _start };

    private (Expression Condition, Expression? ElseExpression) ParseIfExpression()
    {
        var condition = ParseLogicalOr() ?? throw new JinjaException("Expected condition expression");
        var token = ConsumeToken(ElseTokenRegex());
        Expression? elseExpression = null;
        if (!string.IsNullOrEmpty(token))
        {
            elseExpression = ParseExpression(allowIfExpression: false);
            if (elseExpression is null)
                throw new JinjaException("Expected 'else' expression");
        }
        return (condition, elseExpression);
    }

    private Expression? ParseLogicalOr()
    {
        var left = ParseLogicalAnd() ?? throw new JinjaException("Expected left side of 'logical or' expression");
        var location = Location;
        while (!string.IsNullOrEmpty(ConsumeToken(OrTokenRegex())))
        {
            var right = ParseLogicalAnd() ?? throw new JinjaException("Expected right side of 'logical or' expression");
            left = new BinaryOpExpr(location, BinaryOpExpr.Op.Or, left, right);
        }
        return left;
    }

    private Expression? ParseLogicalNot()
    {
        var location = Location;
        var regex = NotTokenRegex();

        if (!string.IsNullOrEmpty(ConsumeToken(regex)))
        {
            var sub = ParseLogicalNot() ?? throw new JinjaException("Expected expression after 'not' keyword");
            return new UnaryOpExpr(location, UnaryOpExpr.Op.LogicalNot, sub);
        }
        return ParseLogicalCompare();
    }


    private Expression? ParseLogicalAnd()
    {
        var left = ParseLogicalNot() ?? throw new JinjaException("Expected left side of 'logical and' expression");
        var location = Location;
        while (!string.IsNullOrEmpty(ConsumeToken(AndTokenRegex())))
        {
            var right = ParseLogicalNot() ?? throw new JinjaException("Expected right side of 'logical and' expression");
            left = new BinaryOpExpr(location, BinaryOpExpr.Op.And, left, right);
        }
        return left;
    }

    private Expression? ParseLogicalCompare()
    {
        var left = ParseStringConcat() ?? throw new JinjaException("Expected left side of 'logical compare' expression");
        string opStr;
        while (!string.IsNullOrEmpty((opStr = ConsumeToken(CompareTokenRegex()))))
        {
            var location = Location;
            if (opStr == "is")
            {
                var negated = !string.IsNullOrEmpty(ConsumeToken(NotTokenRegex()));
                var identifier = ParseIdentifier() ?? throw new JinjaException("Expected identifier after 'is' keyword");
                return new BinaryOpExpr(left.Location, negated ? BinaryOpExpr.Op.IsNot : BinaryOpExpr.Op.Is, left, identifier);
            }
            var right = ParseStringConcat() ?? throw new JinjaException("Expected right side of 'logical compare' expression");
            var op = opStr switch
            {
                "==" => BinaryOpExpr.Op.Eq,
                "!=" => BinaryOpExpr.Op.Ne,
                "<" => BinaryOpExpr.Op.Lt,
                ">" => BinaryOpExpr.Op.Gt,
                "<=" => BinaryOpExpr.Op.Le,
                ">=" => BinaryOpExpr.Op.Ge,
                "in" => BinaryOpExpr.Op.In,
                _ => opStr[..3] == "not" ? BinaryOpExpr.Op.NotIn : throw new JinjaException($"Unknown comparison operator: {opStr}")
            };
            left = new BinaryOpExpr(location, op, left, right);
        }
        return left;
    }

    private List<(string? Name, Expression? Expression)> ParseParameters()
    {
        ConsumeSpaces();
        if (string.IsNullOrEmpty(ConsumeToken("(")))
            throw new JinjaException("Expected opening parenthesis in param list");

        var result = new List<(string? Name, Expression? Expression)>();
        while (_it < _end)
        {
            if (!string.IsNullOrEmpty(ConsumeToken(")")))
                return result;

            var expr = ParseExpression() ?? throw new JinjaException("Expected expression in param list");
            if (expr is VariableExpr ident)
            {
                if (!string.IsNullOrEmpty(ConsumeToken("=")))
                {
                    var value = ParseExpression() ?? throw new JinjaException("Expected expression for named arg");
                    result.Add((ident.Name, value));
                }
                else
                    result.Add((ident.Name, null));
            }
            else
                result.Add((null, expr));
            if (string.IsNullOrEmpty(ConsumeToken(",")))
            {
                if (string.IsNullOrEmpty(ConsumeToken(")")))
                    throw new JinjaException("Expected closing parenthesis in param list");
                return result;
            }
        }
        throw new JinjaException("Expected closing parenthesis in param list");
    }

    private ArgumentsExpression ParseCallArgs()
    {
        ConsumeSpaces();
        if (string.IsNullOrEmpty(ConsumeToken("(")))
            throw new JinjaException("Expected opening parenthesis in call args");

        var result = new ArgumentsExpression();

        while (_it < _end)
        {
            if (!string.IsNullOrEmpty(ConsumeToken(")")))
                return result;

            var expr = ParseExpression() ?? throw new JinjaException("Expected expression in call args");
            if (expr is VariableExpr ident)
            {
                if (!string.IsNullOrEmpty(ConsumeToken("=")))
                {
                    var value = ParseExpression() ?? throw new JinjaException("Expected expression for named arg");
                    result.KwArgs.Add((ident.Name, value));
                }
                else
                    result.Args.Add(expr);
            }
            else
                result.Args.Add(expr);
            if (string.IsNullOrEmpty(ConsumeToken(",")))
            {
                if (string.IsNullOrEmpty(ConsumeToken(")")))
                    throw new JinjaException("Expected closing parenthesis in call args");
                return result;
            }
        }
        throw new JinjaException("Expected closing parenthesis in call args");
    }

    private VariableExpr? ParseIdentifier()
    {
        var identRegex = IdentifierRegex();
        var location = Location;
        var ident = ConsumeToken(identRegex);
        if (string.IsNullOrEmpty(ident))
            return null;
        return new VariableExpr(location, ident);
    }


    private Expression? ParseStringConcat()
    {
        var left = ParseMathPow() ?? throw new JinjaException("Expected left side of 'string concat' expression");
        if (!string.IsNullOrEmpty(ConsumeToken(StringConcatTokenRegex())))
        {
            var right = ParseLogicalAnd() ?? throw new JinjaException("Expected right side of 'string concat' expression");
            left = new BinaryOpExpr(Location, BinaryOpExpr.Op.StrConcat, left, right);
        }
        return left;
    }

    private Expression? ParseMathPow()
    {
        var left = ParseMathPlusMinus() ?? throw new JinjaException("Expected left side of 'math pow' expression");
        while (!string.IsNullOrEmpty(ConsumeToken("**")))
        {
            var right = ParseMathPlusMinus() ?? throw new JinjaException("Expected right side of 'math pow' expression");
            left = new BinaryOpExpr(Location, BinaryOpExpr.Op.MulMul, left, right);
        }
        return left;
    }

    private Expression? ParseMathPlusMinus()
    {
        var left = ParseMathMulDiv() ?? throw new JinjaException("Expected left side of 'math plus/minus' expression");
        string opStr;
        while (!string.IsNullOrEmpty(opStr = ConsumeToken(PlusMinusTokenRegex())))
        {
            var right = ParseMathMulDiv() ?? throw new JinjaException("Expected right side of 'math plus/minus' expression");
            var op = opStr == "+" ? BinaryOpExpr.Op.Add : BinaryOpExpr.Op.Sub;
            left = new BinaryOpExpr(Location, op, left, right);
        }
        return left;
    }

    private Expression? ParseMathMulDiv()
    {
        var left = ParseMathUnaryPlusMinus() ?? throw new JinjaException("Expected left side of 'math mul/div' expression");
        string opStr;
        while (!string.IsNullOrEmpty(opStr = ConsumeToken(MulDivTokenRegex())))
        {
            var right = ParseMathUnaryPlusMinus() ?? throw new JinjaException("Expected right side of 'math mul/div' expression");
            var op = opStr switch
            {
                "*" => BinaryOpExpr.Op.Mul,
                "**" => BinaryOpExpr.Op.MulMul,
                "/" => BinaryOpExpr.Op.Div,
                "//" => BinaryOpExpr.Op.DivDiv,
                _ => BinaryOpExpr.Op.Mod,
            };
            left = new BinaryOpExpr(Location, op, left, right);
        }
        if (!string.IsNullOrEmpty(ConsumeToken("|")))
        {
            var expr = ParseMathMulDiv();
            if (expr is FilterExpr filter)
            {
                filter.Prepend(left);
                return expr;
            }
            else
            {
                if (expr is null)
                    throw new JinjaException("Expected expression after filter");
                return new FilterExpr(Location, [left, expr]);
            }
        }
        return left;
    }


    /// <summary>
    /// Currently unused
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private CallExpr CallFunc(string name, ArgumentsExpression args)
    {
        return new CallExpr(Location, new VariableExpr(Location, name), args);
    }

    private Expression? ParseMathUnaryPlusMinus()
    {
        var opStr = ConsumeToken(UnaryPlusMinTokenRegex());
        var expr = ParseExpansion() ?? throw new JinjaException("Expected expr of 'unary plus/minus/expansion' expression");
        if (!string.IsNullOrEmpty(opStr))
        {
            var op = opStr == "+" ? UnaryOpExpr.Op.Plus : UnaryOpExpr.Op.Minus;
            return new UnaryOpExpr(Location, op, expr);
        }
        return expr;
    }

    private Expression? ParseExpansion()
    {
        var opStr = ConsumeToken(ExpansionTokenRegex());
        var expr = ParseValueExpression();
        if (string.IsNullOrEmpty(opStr))
            return expr;
        if (expr is null)
            throw new JinjaException("Expected expr of 'expansion' expression");
        var op = opStr == "*" ? UnaryOpExpr.Op.Expansion : UnaryOpExpr.Op.ExpansionDict;
        return new UnaryOpExpr(Location, op, expr);
    }

    private Expression? ParseValueExpression()
    {
        Expression? ParseValue()
        {
            var location = Location;
            var constant = ParseConstant();
            if (constant is not null)
                return new LiteralExpr(location, constant);

            var nullToken = ConsumeToken(NullTokenRegex());
            if (!string.IsNullOrEmpty(nullToken))
                return new LiteralExpr(location, Value.Null);

            var identifier = ParseIdentifier();
            if (identifier is not null)
                return identifier;

            var braced = ParseBracedExpressionOrArray();
            if (braced is not null)
                return braced;

            var array = ParseArray();
            if (array is not null)
                return array;

            var dictionary = ParseDictionary();
            if (dictionary is not null)
                return dictionary;

            throw new JinjaException("Expected value expression");
        }

        var value = ParseValue();

        while (_it < _end && ConsumeSpaces() && PeekSymbols(["[", ".", "("]))
        {
            if (!string.IsNullOrEmpty(ConsumeToken("[")))
            {
                var sliceLoc = Location;
                Expression? start = null, end = null, step = null;
                bool hasFirstColon = false, hasSecondColon = false;

                if (!PeekSymbols([":"]))
                    start = ParseExpression();

                if (!string.IsNullOrEmpty(ConsumeToken(":")))
                {
                    hasFirstColon = true;
                    if (!PeekSymbols([":", "]"]))
                        end = ParseExpression();
                    if (!string.IsNullOrEmpty(ConsumeToken(":")))
                    {
                        hasSecondColon = true;
                        if (!PeekSymbols(["]"]))
                            step = ParseExpression();
                    }
                }

                Expression? index;

                if (hasFirstColon || hasSecondColon)
                    index = new SliceExpr(sliceLoc, start!, end!, step!);
                else
                    index = start;
                if (index is null)
                    throw new JinjaException("Empty index in subscript");
                if (string.IsNullOrEmpty(ConsumeToken("]")))
                    throw new JinjaException("Expected closing bracket in subscript");

                value = new SubscriptExpr(value!.Location, value, index);
            }
            else if (!string.IsNullOrEmpty(ConsumeToken(".")))
            {
                var identifier = ParseIdentifier() ?? throw new JinjaException("Expected identifier in subscript");
                ConsumeSpaces();
                if (PeekSymbols(["("]))
                {
                    var callParams = ParseCallArgs();
                    value = new MethodCallExpr(identifier.Location, value!, identifier, callParams);
                }
                else
                {
                    var key = new LiteralExpr(identifier.Location, new Value(identifier.Name));
                    value = new SubscriptExpr(identifier.Location, value!, key);
                }
            }
            else if (PeekSymbols(["("]))
            {
                var callParameters = ParseCallArgs();
                value = new CallExpr(Location, value!, callParameters);
            }
            ConsumeSpaces();
        }
        return value;
    }

    private Expression? ParseBracedExpressionOrArray()
    {
        if (string.IsNullOrEmpty(ConsumeToken("(")))
            return null;

        var expr = ParseExpression() ?? throw new JinjaException("Expected expression in braced expression");
        if (!string.IsNullOrEmpty(ConsumeToken(")")))
            return expr; // Drop the parentheses

        var tuple = new List<Expression> { expr };

        while (_it < _end)
        {
            if (string.IsNullOrEmpty(ConsumeToken(",")))
                throw new JinjaException("Expected comma in tuple");
            var next = ParseExpression() ?? throw new JinjaException("Expected expression in tuple");
            tuple.Add(next);

            if (!string.IsNullOrEmpty(ConsumeToken(")")))
                return new ArrayExpr(Location, tuple);
        }
        throw new JinjaException("Expected closing parenthesis");
    }

    private ArrayExpr? ParseArray()
    {
        if (string.IsNullOrEmpty(ConsumeToken("[")))
            return null;

        var elements = new List<Expression>();
        if (!string.IsNullOrEmpty(ConsumeToken("]")))
            return new ArrayExpr(Location, elements);

        var firstExpr = ParseExpression() ?? throw new JinjaException("Expected first expression in array");
        elements.Add(firstExpr);

        while (_it < _end)
        {
            if (!string.IsNullOrEmpty(ConsumeToken(",")))
            {
                var expr = ParseExpression() ?? throw new JinjaException("Expected expression in array");
                elements.Add(expr);
            }
            else if (!string.IsNullOrEmpty(ConsumeToken("]")))
                return new ArrayExpr(Location, elements);
            else
                throw new JinjaException("Expected comma or closing bracket in array");
        }
        throw new JinjaException("Expected closing bracket");
    }

    private DictExpr? ParseDictionary()
    {
        if (string.IsNullOrEmpty(ConsumeToken("{")))
            return null;

        var elements = new List<(Expression Key, Expression Value)>();
        if (!string.IsNullOrEmpty(ConsumeToken("}")))
            return new DictExpr(Location, elements);

        void ParseKeyValuePair()
        {
            var key = ParseExpression() ?? throw new JinjaException("Expected key in dictionary");
            if (string.IsNullOrEmpty(ConsumeToken(":")))
                throw new JinjaException("Expected colon between key & value in dictionary");
            var value = ParseExpression() ?? throw new JinjaException("Expected value in dictionary");
            elements.Add((key, value));
        }

        ParseKeyValuePair();

        while (_it < _end)
        {
            if (!string.IsNullOrEmpty(ConsumeToken(",")))
                ParseKeyValuePair();
            else if (!string.IsNullOrEmpty(ConsumeToken("}")))
                return new DictExpr(Location, elements);
            else
                throw new JinjaException("Expected comma or closing brace in dictionary");
        }
        throw new JinjaException("Expected closing brace");
    }

    private static SpaceHandling ParsePreSpace(string s)
    {
        return s == "-" ? SpaceHandling.Strip : SpaceHandling.Keep;
    }

    private static SpaceHandling ParsePostSpace(string s)
    {
        return s == "-" ? SpaceHandling.Strip : SpaceHandling.Keep;
    }

    private List<string> ParseVarNames()
    {
        var group = ConsumeTokenGroups(VarNamesRegex());
        if (group.Count == 0)
            throw new JinjaException("Expected variable names");
        var varnames = new List<string>();
        foreach (var varname in group[0].Split(','))
            varnames.Add(StringUtils.Strip(varname));
        return varnames;
    }


    public List<TemplateToken> Tokenize()
    {
        var tokens = new List<TemplateToken>();
        string text;

        try
        {
            while (_it < _end)
            {
                var location = Location;

                // Comment
                var group = ConsumeTokenGroups(CommentTokenRegex(), SpaceHandling.Keep);
                if (group.Count > 0)
                {
                    var preSpace = ParsePreSpace(group[0]);
                    var content = group[1];
                    var postSpace = ParsePostSpace(group[2]);
                    tokens.Add(new CommentTemplateToken(location, preSpace, postSpace, content));
                    continue;
                }

                // Expression
                group = ConsumeTokenGroups(ExprOpenRegex(), SpaceHandling.Keep);
                if (group.Count > 0)
                {
                    var preSpace = ParsePreSpace(group[0]);
                    var expr = ParseExpression();

                    group = ConsumeTokenGroups(ExprCloseRegex());
                    if (group.Count == 0)
                        throw new JinjaException("Expected closing expression tag");

                    var postSpace = ParsePostSpace(group[0]);
                    tokens.Add(new ExpressionTemplateToken(location, preSpace, postSpace, expr!));
                    continue;
                }

                // Block
                group = ConsumeTokenGroups(BlockOpenRegex(), SpaceHandling.Keep);
                if (group.Count > 0)
                {
                    var preSpace = ParsePreSpace(group[0]);

                    SpaceHandling ParseBlockClose()
                    {
                        group = ConsumeTokenGroups(BlockCloseRegex());
                        if (group.Count == 0)
                            throw new JinjaException("Expected closing block tag");
                        return ParsePostSpace(group[0]);
                    }

                    var keyword = ConsumeToken(BlockKeywordTokenRegex());
                    if (string.IsNullOrEmpty(keyword))
                        throw new JinjaException("Expected block keyword");

                    if (keyword == "if")
                    {
                        var condition = ParseExpression() ?? throw new JinjaException("Expected condition in if block");
                        var postSpace = ParseBlockClose();
                        tokens.Add(new IfTemplateToken(location, preSpace, postSpace, condition));
                    }
                    else if (keyword == "elif")
                    {
                        var condition = ParseExpression() ?? throw new JinjaException("Expected condition in elif block");
                        var postSpace = ParseBlockClose();
                        tokens.Add(new ElIfTemplateToken(location, preSpace, postSpace, condition));
                    }
                    else if (keyword == "else")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new ElseTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "endif")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndIfTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "for")
                    {
                        var varnames = ParseVarNames();
                        var inTok = InRegex();
                        if (string.IsNullOrEmpty(ConsumeToken(inTok)))
                            throw new JinjaException("Expected 'in' keyword in for block");
                        var iterable = ParseExpression(allowIfExpression: false) ?? throw new JinjaException("Expected iterable in for block");
                        var ifTok = IfRegex();
                        Expression? condition = null;
                        if (!string.IsNullOrEmpty(ConsumeToken(ifTok)))
                            condition = ParseExpression();

                        var recursiveTok = RecursiveRegex();
                        var recursive = !string.IsNullOrEmpty(ConsumeToken(recursiveTok));

                        var postSpace = ParseBlockClose();
                        tokens.Add(new ForTemplateToken(location, preSpace, postSpace, varnames, iterable, condition ?? new LiteralExpr(location, new Value(true)), recursive));
                    }
                    else if (keyword == "endfor")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndForTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "generation")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new GenerationTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "endgeneration")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndGenerationTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "set")
                    {
                        var namespacedVarRegex = NamespacedVariableRegex();
                        var ns = string.Empty;
                        List<string> varNames = [];
                        Expression? value = null;
                        group = ConsumeTokenGroups(namespacedVarRegex);
                        if (group.Count > 0)
                        {
                            ns = group[0];
                            varNames.Add(group[1]);
                            if (string.IsNullOrEmpty(ConsumeToken("=")))
                                throw new JinjaException("Expected equals sign in set");
                            value = ParseExpression();
                            if (value is null)
                                throw new JinjaException("Expected value in set block");
                        }
                        else
                        {
                            varNames = ParseVarNames();
                            if (!string.IsNullOrEmpty(ConsumeToken("=")))
                            {
                                value = ParseExpression();
                                if (value is null)
                                    throw new JinjaException("Expected value in set block");
                            }
                        }
                        var postSpace = ParseBlockClose();
                        tokens.Add(new SetTemplateToken(location, preSpace, postSpace, ns, varNames, value!));
                    }
                    else if (keyword == "endset")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndSetTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "macro")
                    {
                        var macroname = ParseIdentifier() ?? throw new JinjaException("Expected macro name in macro");
                        var parameters = ParseParameters();
                        var postSpace = ParseBlockClose();
                        tokens.Add(new MacroTemplateToken(location, preSpace, postSpace, macroname, parameters!));
                    }
                    else if (keyword == "endmacro")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndMacroTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "call")
                    {
                        var expr = ParseExpression() ?? throw new JinjaException("Expected expression in call");
                        var postSpace = ParseBlockClose();
                        tokens.Add(new CallTemplateToken(location, preSpace, postSpace, expr));
                    }
                    else if (keyword == "endcall")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndCallTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "filter")
                    {
                        var filter = ParseExpression() ?? throw new JinjaException("Expected expression in filte");
                        var postSpace = ParseBlockClose();
                        tokens.Add(new FilterTemplateToken(location, preSpace, postSpace, filter));
                    }
                    else if (keyword == "endfilter")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new EndFilterTemplateToken(location, preSpace, postSpace));
                    }
                    else if (keyword == "break" || keyword == "continue")
                    {
                        var postSpace = ParseBlockClose();
                        tokens.Add(new LoopControlTemplateToken(location, preSpace, postSpace, keyword == "break" ? LoopControlType.Break : LoopControlType.Continue));
                    }
                    else
                        throw new JinjaException($"Unexpected block: {keyword}");
                    continue;
                }

                // Non-text open
                var match = NonTextOpenRegex().Match(_templateString, _it, _end - _it);
                if (match.Success)
                {
                    if (match.Index == _it)
                    {
                        if (match.Value != "{#")
                            throw new JinjaException("Internal error: Expected a comment");
                        throw new JinjaException("Missing end of comment tag");
                    }
                    var textEnd = match.Index + match.Length;
                    text = _templateString[_it..match.Index];
                    _it = match.Index;
                    tokens.Add(new TextTemplateToken(location, SpaceHandling.Keep, SpaceHandling.Keep, text));
                    continue;
                }

                // Plain text
                text = _templateString[_it.._end];
                _it = _end;
                tokens.Add(new TextTemplateToken(location, SpaceHandling.Keep, SpaceHandling.Keep, text));
            }
            return tokens;
        }
        catch (Exception e)
        {
            throw new JinjaException(e.Message + LocationExtensions.ToString(_templateString, _it), e);
        }
    }





    [GeneratedRegex(@"true\b|True\b|false\b|False\b|None\b")]
    private static partial Regex ConstantTokenRegex();
    [GeneratedRegex(@"\bif\b")]
    private static partial Regex IfTokenRegex();
    [GeneratedRegex(@"else\b")]
    private static partial Regex ElseTokenRegex();
    [GeneratedRegex(@"or\b")]
    private static partial Regex OrTokenRegex();
    [GeneratedRegex(@"not\b")]
    private static partial Regex NotTokenRegex();
    [GeneratedRegex(@"and\b")]
    private static partial Regex AndTokenRegex();
    [GeneratedRegex(@"==|!=|<=?|>=?|in\b|is\b|not\s+in\b")]
    private static partial Regex CompareTokenRegex();
    [GeneratedRegex(@"((?!(?:not|is|and|or|del)\b)[a-zA-Z_]\w*)")]
    private static partial Regex IdentifierRegex();
    [GeneratedRegex(@"~(?!\})")]
    private static partial Regex StringConcatTokenRegex();
    [GeneratedRegex(@"\+|-(?![}%#]\})")]
    private static partial Regex PlusMinusTokenRegex();
    [GeneratedRegex(@"\*\*?|//?|%(?!\})")]
    private static partial Regex MulDivTokenRegex();
    [GeneratedRegex(@"\*\*?")]
    private static partial Regex ExpansionTokenRegex();
    [GeneratedRegex(@"null\b")]
    private static partial Regex NullTokenRegex();
    [GeneratedRegex(@"((?:\w+)(?:\s*,\s*(?:\w+))*)\s*")]
    private static partial Regex VarNamesRegex();
    [GeneratedRegex(@"\{#([-~]?)([\s\S]*?)([-~]?)#\}")]
    private static partial Regex CommentTokenRegex();
    [GeneratedRegex(@"\{\{([-~])?")]
    private static partial Regex ExprOpenRegex();
    [GeneratedRegex(@"^\{%([-~])?\s*")]
    private static partial Regex BlockOpenRegex();
    [GeneratedRegex(@"(if|else|elif|endif|for|endfor|generation|endgeneration|set|endset|block|endblock|macro|endmacro|filter|endfilter|break|continue|call|endcall)\b")]
    private static partial Regex BlockKeywordTokenRegex();
    [GeneratedRegex(@"\{\{|\{%|\{#")]
    private static partial Regex NonTextOpenRegex();
    [GeneratedRegex(@"\s*([-~])?\}\}")]
    private static partial Regex ExprCloseRegex();
    [GeneratedRegex(@"\s*([-~])?%\}")]
    private static partial Regex BlockCloseRegex();
    [GeneratedRegex(@"(\w+)\s*\.\s*(\w+)")]
    private static partial Regex NamespacedVariableRegex();
    [GeneratedRegex(@"recursive\b")]
    private static partial Regex RecursiveRegex();
    [GeneratedRegex(@"if\b")]
    private static partial Regex IfRegex();
    [GeneratedRegex(@"in\b")]
    private static partial Regex InRegex();
    [GeneratedRegex(@"\+|-(?![}%#]\})")]
    private static partial Regex UnaryPlusMinTokenRegex();
}
