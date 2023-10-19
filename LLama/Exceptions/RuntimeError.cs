using System;

namespace LLama.Exceptions;

/// <summary>
/// Base class for LLamaSharp runtime errors (i.e. errors produced by llama.cpp, converted into exceptions)
/// </summary>
public class RuntimeError
    : Exception
{
    /// <summary>
    /// Create a new RuntimeError
    /// </summary>
    /// <param name="message"></param>
    public RuntimeError(string message)
        : base(message)
    {

    }
}