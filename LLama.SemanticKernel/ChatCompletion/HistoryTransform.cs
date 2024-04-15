﻿using LLama.Common;
using System.Text;
using static LLama.LLamaTransforms;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

/// <summary>
/// Default HistoryTransform Patch
/// </summary>
public class HistoryTransform : DefaultHistoryTransform<ChatHistory>
{
    /// <inheritdoc/>
    public string HistoryToText(global::LLama.Common.ChatHistory history)
    {
        return base.HistoryToText(history) + $"{AuthorRole.Assistant}: ";
    }
}
