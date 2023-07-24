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
        /// <summary>
        /// The initial state of the model
        /// </summary>
        public State OriginalState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Params"></param>
        /// <param name="encoding"></param>
        public ResettableLLamaModel(ModelParams Params, string encoding = "UTF-8") : base(Params, encoding)
        {
            OriginalState = GetState();
        }

        /// <summary>
        /// Reset the state to the initial state.
        /// </summary>
        public void Reset()
        {
            LoadState(OriginalState);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            OriginalState.Dispose();

            base.Dispose();
        }
    }
}
