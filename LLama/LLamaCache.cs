using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama
{
    using llama_token = Int32;
    /// <summary>
    /// Cache for a llama.cpp model.
    /// </summary>
    public class LLamaCache
    {
        private Dictionary<llama_token[], LinkedListNode<KeyValuePair<llama_token[], LLamaState>>> _cacheState;
        private LinkedList<KeyValuePair<llama_token[], LLamaState>> _cacheList;
        private int _capacity;

        public int CacheSize
        {
            get
            {
                return _cacheState.Values.Select(s => s.Value.Value.Size).Sum();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity">The max capacity (bytes).</param>
        public LLamaCache(int capacity = 2 << 30)
        {
            _cacheState = new();
            _cacheList = new();
            _capacity = capacity;
        }

        public LLamaState this[llama_token[] key]
        {
            get
            {
                var prefixKey = FindLongestPrefixKey(key);
                if(prefixKey is null)
                {
                    throw new KeyNotFoundException();
                }
                var value = _cacheState[prefixKey];
                MoveNodeToEnd(prefixKey);
                return value.Value.Value;
            }
            set
            {
                var node = _cacheList.AddLast(new KeyValuePair<llama_token[], LLamaState>(key, value));
                _cacheState[key] = node;
                while(CacheSize > _capacity && _cacheList.Count > 0)
                {
                    var topop = _cacheList.First;
                    _cacheState.Remove(topop.Value.Key);
                    _cacheList.RemoveFirst();
                }
            }
        }

        public bool Contains(llama_token[] key)
        {
            return FindLongestPrefixKey(key) is not null;
        }

        private llama_token[]? FindLongestPrefixKey(llama_token[] key)
        {
            int minLen = 0;
            llama_token[]? minKey = null;
            var keys = _cacheState.Keys.Select(k => (k, LLamaModel.LongestTokenPrefix(k, key)));
            foreach(var (k, prefixLen) in keys)
            {
                if(prefixLen > minLen)
                {
                    minLen = prefixLen;
                    minKey = k;
                }
            }
            return minKey;
        }

        private void MoveNodeToEnd(llama_token[] key)
        {
            if (!_cacheState.TryGetValue(key, out var node))
            {
                return;
            }

            _cacheState.Remove(key);
            _cacheList.Remove(node);

            var newNode = _cacheList.AddLast(new KeyValuePair<llama_token[], LLamaState>(key, node.Value.Value));
            _cacheState.Add(key, newNode);
        }
    }
}
