using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Runner.LLamaCpp
{
    /// <summary>
    /// Since the native API will return <see cref="Span{T}"/> for logits, 
    /// we only get it when it's actually needed.
    /// </summary>
    public class LogitsGenerator
    {
        private int _pos;

        private LLamaContext _context;

        public LogitsGenerator(int pos, LLamaContext context)
        {
            _pos = pos;
            _context = context;
        }

        public Span<float> GetLogits()
        {
            return _context.NativeHandle.GetLogitsIth(_pos);
        }
    }
}
