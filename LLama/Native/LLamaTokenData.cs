using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LLamaTokenData
    {
        /// <summary>
        /// token id
        /// </summary>
        public int id;
        /// <summary>
        /// log-odds of the token
        /// </summary>
        public float logit;
        /// <summary>
        /// probability of the token
        /// </summary>
        public float p;

        public LLamaTokenData(int id, float logit, float p)
        {
            this.id = id;
            this.logit = logit;
            this.p = p;
        }
    }
}
