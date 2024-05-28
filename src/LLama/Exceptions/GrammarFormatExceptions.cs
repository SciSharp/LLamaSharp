using System;

namespace LLama.Exceptions;

/// <summary>
/// Base class for all grammar exceptions
/// </summary>
public abstract class GrammarFormatException
    : Exception
{
    internal GrammarFormatException(string message)
        : base(message)
    {
    }
}


/// <summary>
/// An incorrect number of characters were encountered while parsing a hex literal
/// </summary>
public class GrammarUnexpectedHexCharsCount
    : GrammarFormatException
{
    internal GrammarUnexpectedHexCharsCount(int size, string source)
        : base($"Expecting {size} hex chars at {source}")
    {
    }
}

/// <summary>
/// Failed to parse a "name" element when one was expected
/// </summary>
public class GrammarExpectedName
    : GrammarFormatException
{
    internal GrammarExpectedName(string source)
        : base($"Expecting name at {source}")
    {
    }
}

/// <summary>
/// An unexpected character was encountered after an escape sequence
/// </summary>
public class GrammarUnknownEscapeCharacter
    : GrammarFormatException
{
    internal GrammarUnknownEscapeCharacter(string source)
        : base($"Unknown escape at {source}")
    {
    }
}

/// <summary>
/// End-of-file was encountered while parsing
/// </summary>
public class GrammarUnexpectedEndOfInput
    : GrammarFormatException
{
    internal GrammarUnexpectedEndOfInput()
        : base("Unexpected end of input")
    {
    }
}

/// <summary>
/// A specified string was expected when parsing
/// </summary>
public class GrammarExpectedNext
    : GrammarFormatException
{
    internal GrammarExpectedNext(string expected, string source)
        : base($"Expected '{expected}' at {source}")
    {
    }
}

/// <summary>
/// A specified character was expected to preceded another when parsing
/// </summary>
public class GrammarExpectedPrevious
    : GrammarFormatException
{
    internal GrammarExpectedPrevious(string expected, string source)
        : base($"Expecting preceding item to be '{expected}' at {source}")
    {
    }
}


/// <summary>
/// A CHAR_ALT was created without a preceding CHAR element
/// </summary>
public class GrammarUnexpectedCharAltElement
    : GrammarFormatException
{
    internal GrammarUnexpectedCharAltElement(string ruleId, int index)
        : base($"LLamaGrammarElementType.CHAR_ALT without preceding char: {ruleId},{index}")
    {
    }
}

/// <summary>
/// A CHAR_RNG was created without a preceding CHAR element
/// </summary>
public class GrammarUnexpectedCharRngElement
    : GrammarFormatException
{
    internal GrammarUnexpectedCharRngElement(string ruleId, int index)
        : base($"LLamaGrammarElementType.CHAR_RNG_UPPER without preceding char: {ruleId},{index}")
    {
    }
}

/// <summary>
/// An END was encountered before the last element
/// </summary>
public class GrammarUnexpectedEndElement
    : GrammarFormatException
{
    internal GrammarUnexpectedEndElement(string ruleId, int index)
        : base($"Unexpected LLamaGrammarElementType.END: {ruleId},{index}")
    {
    }
}