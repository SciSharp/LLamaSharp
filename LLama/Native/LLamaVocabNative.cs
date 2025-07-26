namespace LLama.Native;

/// <summary>
/// C# equivalent of llama_vocab struct. This struct is an opaque type, with no fields in the API and is only used for typed pointers.
/// </summary>
internal struct LLamaVocabNative
{
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe int llama_vocab_n_tokens(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaVocabType llama_vocab_type(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe byte* llama_vocab_get_text(LLamaVocabNative* vocab, LLamaToken token);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe float llama_vocab_get_score(LLamaVocabNative* vocab, LLamaToken token);

    /// <summary>
    /// Get attributes for a specific token
    /// </summary>
    /// <param name="vocab"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaTokenAttr llama_vocab_get_attr(LLamaVocabNative* vocab, LLamaToken token);

    /// <summary>
    /// Check if the token is supposed to end generation (end-of-generation, eg. EOS, EOT, etc.)
    /// </summary>
    /// <param name="vocab"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern unsafe bool llama_vocab_is_eog(LLamaVocabNative* vocab, LLamaToken token);

    /// <summary>
    /// Identify if Token Id is a control token or a render-able token
    /// </summary>
    /// <param name="vocab"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern unsafe bool llama_vocab_is_control(LLamaVocabNative* vocab, LLamaToken token);

    /// <summary>
    /// beginning-of-sentence
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_bos(LLamaVocabNative* vocab);

    /// <summary>
    /// end-of-sentence
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_eos(LLamaVocabNative* vocab);

    /// <summary>
    /// end-of-turn
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_eot(LLamaVocabNative* vocab);

    /// <summary>
    /// sentence separator
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_sep(LLamaVocabNative* vocab);

    /// <summary>
    /// next-line
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_nl(LLamaVocabNative* vocab);

    /// <summary>
    /// padding
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_pad(LLamaVocabNative* vocab);

    /// <summary>
    /// mask
    /// </summary>
    /// <param name="vocab"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_mask(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_pre(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_suf(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_mid(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_pad(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_rep(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe LLamaToken llama_vocab_fim_sep(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern unsafe bool llama_vocab_get_add_bos(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern unsafe bool llama_vocab_get_add_eos(LLamaVocabNative* vocab);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern unsafe bool llama_vocab_get_add_sep(LLamaVocabNative* vocab);
}