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

/// <summary>
/// Loading model weights failed
/// </summary>
public class LoadWeightsFailedException
    : RuntimeError
{
    /// <summary>
    /// The model path which failed to load
    /// </summary>
    public string ModelPath { get; }

    /// <inheritdoc />
    public LoadWeightsFailedException(string modelPath)
        : base($"Failed to load model '{modelPath}'")
    {
        ModelPath = modelPath;
    }
}