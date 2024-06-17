using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace LLama.Model;

// This is for demo purposes - it can be finalized later
internal class HuggingFaceModelRepo(ILogger<HuggingFaceModelRepo> logger, 
    HttpClient hfClient) : IModelSourceRepo
{
    private readonly ILogger<HuggingFaceModelRepo> _logger = logger;
    private readonly HttpClient _hfClient = hfClient;

    // https://huggingface.co/leliuga/all-MiniLM-L12-v2-GGUF/resolve/main/all-MiniLM-L12-v2.Q8_0.gguf
    private readonly HashSet<string> _hfModelUri = [];

    public void AddSource(string source)
    {
        if (!Uri.IsWellFormedUriString(source, UriKind.Absolute))
        {
            Trace.TraceWarning("URI is not a valid HuggingFace URL");
        }

        // TODO: call HF to check model exists 
        // TODO: Get metadata about model
        _hfModelUri.Add(source);
    }

    public IEnumerable<string> ListSources() => _hfModelUri;

    public void RemoveAllSources()
    {
        _hfModelUri.Clear();
    }

    public bool RemoveSource(string source)
    {
        return _hfModelUri.Remove(source);
    }

    public bool TryGetModelFileMetadata(string modelFileName, out ModelFileMetadata modelMeta)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ModelFileMetadata> GetAvailableModels()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ModelFileMetadata> GetAvailableModelsFromSource(string source)
    {
        throw new NotImplementedException();
    }
}
