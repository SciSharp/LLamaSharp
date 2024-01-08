using LLama.Abstractions;
using LLama.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Extensions
{
    /// <summary>
    /// Extension methods for generation control
    /// </summary>
    public static class GenerationControlExtensions
    {
        public static bool ShouldStopGeneration(this IGenerationControl control, LLamaContext context, IInferenceParams inferenceParams, IEnumerable<int> lastOutputIds)
        {
            foreach (var id in lastOutputIds)
            {
                if(control.ShouldStopGeneration(context, inferenceParams, id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
