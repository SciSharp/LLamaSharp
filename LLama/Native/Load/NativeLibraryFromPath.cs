using LLama.Abstractions;
using System.Collections.Generic;

namespace LLama.Native
{
    /// <summary>
    /// A native library specified with a local file path.
    /// </summary>
    public class NativeLibraryFromPath: INativeLibrary
    {
        private string _path;

        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public NativeLibraryFromPath(string path)
        {
            _path = path;
        }
        
        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            return [_path];
        }
    }
}
