using LLama.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Native
{
    /// <summary>
    /// When you are using .NET standard2.0, dynamic native library loading is not supported.
    /// This class will be returned in <see cref="NativeLibraryConfig.DryRun(out INativeLibrary)"/>.
    /// </summary>
    public class UnknownNativeLibrary: INativeLibrary
    {
        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata => null;

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
        {
            throw new NotSupportedException("This class is only a placeholder and should not be used to load native library.");
        }
    }
}
