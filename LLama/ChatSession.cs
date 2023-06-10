using LLama.Abstractions.Params;
using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LLama
{
    using llama_token = Int32;
    //public class ChatHistoryEntry
    //{
    //    public string Role { get; set; }
    //    public string Text { get; set; }
    //}

    //public class ChatMetadata
    //{
    //    public string Prompt { get; set; } = "Prompt";
    //    public IEnumerable<string>? AntiPrompts { get; set; } = null;
    //    public string User { get; set; } = "User";
    //    public string Assistant { get; set; } = "Assistant";

    //    public ChatMetadata SetPrompt(string v)
    //    {
    //        Prompt = v;
    //        return this;
    //    }

    //    public ChatMetadata SetUserName(string v)
    //    {
    //        User = v;
    //        return this;
    //    }

    //    public ChatMetadata SetAssistantName(string v)
    //    {
    //        Assistant = v;
    //        return this;
    //    }

    //    public ChatMetadata WithPromptFromFile(string filename)
    //    {
    //        Prompt = System.IO.File.ReadAllText(filename);
    //        return this;
    //    }
    //}

    //public class ChatSession
    //{
    //    private LLamaModel _model;
    //    private ChatMetadata _metadata;

    //    public List<ChatHistoryEntry> ChatHistory { get; } = new();
    //    public ChatSession(LLamaModel model, ChatMetadata? metadata = null)
    //    {
    //        _model = model;
    //        if (metadata == null) metadata = new ChatMetadata();
    //        _metadata = metadata;

    //        if (_metadata.Prompt != "")
    //        {
    //            ChatHistory.Add(new ChatHistoryEntry() { Role = "", Text = _metadata.Prompt });
    //        }
    //    }

    //    string _formatChatHistory(List<ChatHistoryEntry> history)
    //    {
    //        StringBuilder sb = new();
    //        foreach (var entry in history)
    //        {
    //            if (entry.Role == "")
    //            {
    //                sb.Append($"{entry.Text}\n");
    //                continue;
    //            }
    //            sb.Append($"{entry.Role}: {entry.Text}\n");
    //        }
    //        sb.Append($"{_metadata.Assistant}: ");
    //        return sb.ToString();
    //    }

    //    public IEnumerable<string> Chat(string text)
    //    {
    //        ChatHistory.Add(new ChatHistoryEntry() { Role = "User", Text = text });
    //        string totalResponse = "";
    //        //foreach (var response in _model.GenerateResult(_formatChatHistory(ChatHistory), null, _metadata.AntiPrompts))
    //        //{
    //        //    totalResponse += response;
    //        //    yield return response;
    //        //}
    //        ChatHistory.Add(new ChatHistoryEntry() { Role = "Assistant", Text = totalResponse });
    //    }
    //}

    

}