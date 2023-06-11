using LLama.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama
{
    /// <summary>
    /// A LLamaModel what could be reset. Note that using this class will consume about 10% more memories.
    /// </summary>
    public class ResettableLLamaModel : LLamaModel
    {
        public byte[] OriginalState { get; set; }
        public ResettableLLamaModel(ModelParams Params, string encoding = "UTF-8") : base(Params, encoding)
        {
            OriginalState = GetStateData();
        }

        public void Reset()
        {
            LoadState(OriginalState);
        }
    }
}
