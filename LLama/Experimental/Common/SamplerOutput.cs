using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// For each sequence group, we generate a list of SequenceOutput object,
    /// each of which contains one possible candidate for the next token.
    /// 
    /// This datastructure implements methods so it can be used like a list.
    /// </summary>
    public class SamplerOutput: IList<SequenceGroupOutput>
    {
        /// <summary>
        /// The list of <see cref="SequenceGroupOutput"/> objects, which are the outputs of the LLM model.
        /// </summary>
        public List<SequenceGroupOutput> Outputs { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputs"></param>
        public SamplerOutput(List<SequenceGroupOutput> outputs)
        {
            Outputs = outputs;
        }

        #region IList Implementation
        /// <inheritdoc/>
        public SequenceGroupOutput this[int index] { get => Outputs[index]; set => Outputs[index] = value; }
        /// <inheritdoc/>
        public int Count => Outputs.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => false;
        /// <inheritdoc/>
        public void Add(SequenceGroupOutput item)
        {
            Outputs.Add(item);
        }
        /// <inheritdoc/>
        public void Clear()
        {
            Outputs.Clear();
        }
        /// <inheritdoc/>
        public bool Contains(SequenceGroupOutput item)
        {
            return Outputs.Contains(item);
        }
        /// <inheritdoc/>
        public void CopyTo(SequenceGroupOutput[] array, int arrayIndex)
        {
            Outputs.CopyTo(array, arrayIndex);
        }
        /// <inheritdoc/>
        public IEnumerator<SequenceGroupOutput> GetEnumerator()
        {
            return Outputs.GetEnumerator();
        }
        /// <inheritdoc/>
        public int IndexOf(SequenceGroupOutput item)
        {
            return Outputs.IndexOf(item);
        }
        /// <inheritdoc/>
        public void Insert(int index, SequenceGroupOutput item)
        {
            Outputs.Insert(index, item);
        }
        /// <inheritdoc/>
        public bool Remove(SequenceGroupOutput item)
        {
            return Outputs.Remove(item);
        }
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Outputs.RemoveAt(index);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Outputs.GetEnumerator();
        }
        #endregion
    }
}
