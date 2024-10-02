namespace LLama.Sampling;

/// <summary>
/// A grammar in GBNF form
/// </summary>
/// <param name="Gbnf"></param>
/// <param name="Root"></param>
public record Grammar(string Gbnf, string Root);