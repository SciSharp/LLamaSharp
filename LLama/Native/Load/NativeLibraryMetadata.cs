
namespace LLama.Native
{
    /// <summary>
    /// Information of a native library file.
    /// </summary>
    /// <param name="NativeLibraryName">Which kind of library it is.</param>
    /// <param name="UseCuda">Whether it's compiled with cublas.</param>
    /// <param name="AvxLevel">Which AvxLevel it's compiled with.</param>
    public record class NativeLibraryMetadata(NativeLibraryName NativeLibraryName, bool UseCuda, AvxLevel AvxLevel)
    {
        public override string ToString()
        {
            return $"(NativeLibraryName: {NativeLibraryName}, UseCuda: {UseCuda}, AvxLevel: {AvxLevel})";
        }
    }

    /// <summary>
    /// Avx support configuration
    /// </summary>
    public enum AvxLevel
    {
        /// <summary>
        /// No AVX
        /// </summary>
        None,

        /// <summary>
        /// Advanced Vector Extensions (supported by most processors after 2011)
        /// </summary>
        Avx,

        /// <summary>
        /// AVX2 (supported by most processors after 2013)
        /// </summary>
        Avx2,

        /// <summary>
        /// AVX512 (supported by some processors after 2016, not widely supported)
        /// </summary>
        Avx512,
    }
}
