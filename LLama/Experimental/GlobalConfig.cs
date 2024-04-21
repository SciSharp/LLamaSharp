using LLama.Experimental.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental
{
    /// <summary>
    /// Global configuration for LLamaSharp.
    /// </summary>
    public class GlobalConfig
    {
        public static ISamplingMethod DefaultSamplingMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static IStoppingCriteria DefaultStoppingCriteria
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
