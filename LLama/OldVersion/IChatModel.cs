﻿using System;
using System.Collections.Generic;

#pragma warning disable
// ReSharper disable all

namespace LLama.OldVersion
{
    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public interface IChatModel
    {
        string Name { get; }
        IEnumerable<string> Chat(string text, string? prompt = null, string encoding = "UTF-8");
        /// <summary>
        /// Init a prompt for chat and automatically produce the next prompt during the chat.
        /// </summary>
        /// <param name="prompt"></param>
        void InitChatPrompt(string prompt, string encoding = "UTF-8");
        void InitChatAntiprompt(string[] antiprompt);
    }
}
