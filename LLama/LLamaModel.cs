using LLama.Exceptions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using LLama.Common;
using System.Runtime.InteropServices;
using LLama.Extensions;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.Collections.Concurrent;

namespace LLama
{
    public class LLamaModel : IDisposable
    {
        // TODO: expose more properties.
        ILLamaLogger? _logger;
        SafeLlamaModelHandle _model;
        /// <summary>
        /// The model params set for this model.
        /// </summary>
        public ModelParams Params { get; set; }
        /// <summary>
        /// The native handle, which is used to be passed to the native APIs. Please avoid using it 
        /// unless you know what is the usage of the Native API.
        /// </summary>
        public SafeLlamaModelHandle NativeHandle => _model;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelParams">Model params.</param>
        /// <param name="logger">The logger.</param>
        public LLamaModel(ModelParams modelParams, ILLamaLogger? logger = null)
        {
            _logger = logger;
            Params = modelParams;
            _logger?.Log(nameof(LLamaModelContext), $"Initializing LLama model with params: {modelParams}", ILLamaLogger.LogLevel.Info);

            var contextParams = Utils.CreateContextParams(modelParams);
            _model = SafeLlamaModelHandle.LoadFromFile(modelParams.ModelPath, contextParams);
            if (!string.IsNullOrEmpty(modelParams.LoraAdapter))
                _model.ApplyLoraFromFile(modelParams.LoraAdapter, modelParams.LoraBase, modelParams.Threads);
        }


        /// <inheritdoc />
        public virtual void Dispose()
        {
            _model.Dispose();
        }
    }
}
