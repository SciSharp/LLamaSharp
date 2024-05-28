using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// LLama performance information
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaTimings
{
    /// <summary>
    /// Timestamp when reset was last called
    /// </summary>
    private double t_start_ms;

    /// <summary>
    /// Timestamp when these timings were read
    /// </summary>
    private double t_end_ms;

    /// <summary>
    /// Loading milliseconds
    /// </summary>
    private double t_load_ms;
    
    /// <summary>
    /// Total sampling milliseconds
    /// </summary>
    private double t_sample_ms;

    /// <summary>
    /// total milliseconds spent prompt processing
    /// </summary>
    private double t_p_eval_ms;

    /// <summary>
    /// Total milliseconds in eval/decode calls
    /// </summary>
    private double t_eval_ms;

    /// <summary>
    /// number of tokens sampled
    /// </summary>
    private int n_sample;

    /// <summary>
    /// number of tokens in eval calls for the prompt (with batch size > 1)
    /// </summary>
    private int n_p_eval;

    /// <summary>
    /// number of eval calls
    /// </summary>
    private int n_eval;
    



    /// <summary>
    /// Timestamp when reset was last called
    /// </summary>
    public readonly TimeSpan ResetTimestamp => TimeSpan.FromMilliseconds(t_start_ms);

    /// <summary>
    /// Timestamp when these timings were read
    /// </summary>
    public readonly TimeSpan ReadTimestamp => TimeSpan.FromMilliseconds(t_start_ms);

    /// <summary>
    /// Time spent loading
    /// </summary>
    public readonly TimeSpan Loading => TimeSpan.FromMilliseconds(t_load_ms);

    /// <summary>
    /// Time spent sampling
    /// </summary>
    public readonly TimeSpan Sampling => TimeSpan.FromMilliseconds(t_load_ms);

    /// <summary>
    /// total milliseconds spent prompt processing
    /// </summary>
    public TimeSpan PromptEval => TimeSpan.FromMilliseconds(t_p_eval_ms);

    /// <summary>
    /// Total milliseconds in eval/decode calls
    /// </summary>
    public readonly TimeSpan Eval => TimeSpan.FromMilliseconds(t_eval_ms);

    /// <summary>
    /// Total number of tokens sampled
    /// </summary>
    public readonly int TokensSampled => n_sample;

    /// <summary>
    /// number of tokens in eval calls for the prompt (with batch size > 1)
    /// </summary>
    public readonly int PrompTokensEvaluated => n_p_eval;

    /// <summary>
    /// number of eval calls
    /// </summary>
    public readonly int TokensEvaluated => n_p_eval;
}