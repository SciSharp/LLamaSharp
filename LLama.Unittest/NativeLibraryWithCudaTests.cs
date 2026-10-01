using LLama.Native;
using System.Runtime.InteropServices;

namespace LLama.Unittest
{
    public class NativeLibraryWithCudaTests
    {
        [Fact]
        public void SkipCheckUnknownMajorOnLinuxYieldsCuda13Then12Then11()
        {
            var library = new NativeLibraryWithCuda(-1, NativeLibraryName.LLama, AvxLevel.Avx2, true);
            var systemInfo = new SystemInfo(OSPlatform.Linux, -1, null);

            var paths = library.Prepare(systemInfo, null).ToArray();

            Assert.Equal(3, paths.Length);
            Assert.Contains("cuda13", paths[0]);
            Assert.Contains("cuda12", paths[1]);
            Assert.Contains("cuda11", paths[2]);
        }

        [Fact]
        public void DetectedMajor13OnWindowsYieldsCuda13Only()
        {
            var library = new NativeLibraryWithCuda(13, NativeLibraryName.LLama, AvxLevel.Avx2, false);
            var systemInfo = new SystemInfo(OSPlatform.Windows, 13, null);

            var paths = library.Prepare(systemInfo, null).ToArray();

            Assert.Single(paths);
            Assert.Contains("cuda13", paths[0]);
            Assert.DoesNotContain("cuda12", paths[0]);
            Assert.DoesNotContain("cuda11", paths[0]);
        }

        [Fact]
        public void DetectedMajor12YieldsCuda12Only()
        {
            var library = new NativeLibraryWithCuda(12, NativeLibraryName.LLama, AvxLevel.Avx2, false);
            var systemInfo = new SystemInfo(OSPlatform.Linux, 12, null);

            var paths = library.Prepare(systemInfo, null).ToArray();

            Assert.Single(paths);
            Assert.Contains("cuda12", paths[0]);
            Assert.DoesNotContain("cuda13", paths[0]);
            Assert.DoesNotContain("cuda11", paths[0]);
        }

        [Fact]
        public void SkipCheckFalseUnknownMajorYieldsEmpty()
        {
            var library = new NativeLibraryWithCuda(-1, NativeLibraryName.LLama, AvxLevel.Avx2, false);
            var systemInfo = new SystemInfo(OSPlatform.Linux, -1, null);

            var paths = library.Prepare(systemInfo, null).ToArray();

            Assert.Empty(paths);
        }
    }
}
