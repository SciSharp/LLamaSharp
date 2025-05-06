using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Native
{
    /// <summary>
    /// Helper for creating and managing tensor buffer overrides
    /// </summary>
    public class LLamaTensorBufferOverrideHelper : IDisposable
    {
        private readonly List<IntPtr> _allocatedMemory = new();
        private readonly List<LLamaModelTensorBufferOverride> _overrides = new();
        private IntPtr _overrideArray = IntPtr.Zero;
        private readonly Dictionary<string, IntPtr> _bufferTypeCache = new();

        /// <summary>
        /// Get all available buffer types
        /// </summary>
        /// <returns>Dictionary mapping buffer type names to their handles</returns>
        public Dictionary<string, IntPtr> GetAvailableBufferTypes()
        {
            var result = new Dictionary<string, IntPtr>();
            
            nuint count = NativeApi.ggml_backend_dev_count();
            for (nuint i = 0; i < count; i++)
            {
                IntPtr dev = NativeApi.ggml_backend_dev_get(i);
                IntPtr buft = NativeApi.ggml_backend_dev_buffer_type(dev);
                
                if (buft != IntPtr.Zero)
                {
                    IntPtr namePtr = NativeApi.ggml_backend_buft_name(buft);
                    string name = Marshal.PtrToStringAnsi(namePtr) ?? string.Empty;
                    
                    if (!string.IsNullOrEmpty(name))
                    {
                        result[name] = buft;
                        _bufferTypeCache[name] = buft;
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// Add a tensor buffer override
        /// </summary>
        /// <param name="pattern">Tensor name pattern to match</param>
        /// <param name="bufferTypeName">Name of the buffer type to use</param>
        /// <returns>True if the override was added successfully</returns>
        public bool AddOverride(string pattern, string bufferTypeName)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(bufferTypeName))
                return false;

            // Get all buffer types if cache is empty
            if (_bufferTypeCache.Count == 0)
            {
                GetAvailableBufferTypes();
            }

            // Check if we have this buffer type
            if (!_bufferTypeCache.TryGetValue(bufferTypeName, out IntPtr bufferType))
                return false;

            // Allocate memory for the pattern string and keep track of it
            byte[] patternBytes = Encoding.UTF8.GetBytes(pattern + "\0");
            IntPtr patternPtr = Marshal.AllocHGlobal(patternBytes.Length);
            Marshal.Copy(patternBytes, 0, patternPtr, patternBytes.Length);
            _allocatedMemory.Add(patternPtr);

            // Create the override
            var @override = new LLamaModelTensorBufferOverride
            {
                Pattern = patternPtr,
                BufferType = bufferType
            };

            _overrides.Add(@override);
            return true;
        }

        /// <summary>
        /// Apply the overrides to model parameters
        /// </summary>
        /// <param name="modelParams">Model parameters to update</param>
        public unsafe void ApplyToModelParams(ref LLamaModelParams modelParams)
        {
            if (_overrides.Count == 0)
            {
                modelParams.tensor_buft_overrides = IntPtr.Zero;
                return;
            }

            // Free previous array if it exists
            if (_overrideArray != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_overrideArray);
            }

            // Allocate memory for the array + null terminator
            int size = Marshal.SizeOf<LLamaModelTensorBufferOverride>() * (_overrides.Count + 1);
            _overrideArray = Marshal.AllocHGlobal(size);
            _allocatedMemory.Add(_overrideArray);

            // Copy overrides to array
            for (int i = 0; i < _overrides.Count; i++)
            {
                IntPtr elemPtr = IntPtr.Add(_overrideArray, i * Marshal.SizeOf<LLamaModelTensorBufferOverride>());
                Marshal.StructureToPtr(_overrides[i], elemPtr, false);
            }

            // Add null terminator
            IntPtr nullTermPtr = IntPtr.Add(_overrideArray, _overrides.Count * Marshal.SizeOf<LLamaModelTensorBufferOverride>());
            Marshal.StructureToPtr(new LLamaModelTensorBufferOverride { Pattern = IntPtr.Zero, BufferType = IntPtr.Zero }, nullTermPtr, false);

            // Update model params
            modelParams.tensor_buft_overrides = _overrideArray;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (IntPtr ptr in _allocatedMemory)
            {
                Marshal.FreeHGlobal(ptr);
            }
            _allocatedMemory.Clear();
            _overrides.Clear();
            _overrideArray = IntPtr.Zero;
        }
    }
}
