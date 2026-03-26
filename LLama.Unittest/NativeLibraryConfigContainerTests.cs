using System.Reflection;
using System.Runtime.InteropServices;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Unittest
{
    public class NativeLibraryConfigContainerTests
    {
        [Fact]
        public void DryRunPreservesLoadedLibraries()
        {
            var libraryPath = FindLoadableNativeLibraryPath();
            var selectingPolicy = new StubSelectingPolicy(new StubNativeLibrary(libraryPath));

            var llamaConfig = CreateConfig(NativeLibraryName.LLama).WithSelectingPolicy(selectingPolicy);
            var mtmdConfig = CreateConfig(NativeLibraryName.Mtmd).WithSelectingPolicy(selectingPolicy);
            var container = new NativeLibraryConfigContainer(llamaConfig, mtmdConfig);

            var success = container.DryRun(out var loadedLLamaNativeLibrary, out var loadedMtmdNativeLibrary);

            Assert.True(success);
            Assert.NotNull(loadedLLamaNativeLibrary);
            Assert.NotNull(loadedMtmdNativeLibrary);
            Assert.IsType<StubNativeLibrary>(loadedLLamaNativeLibrary);
            Assert.IsType<StubNativeLibrary>(loadedMtmdNativeLibrary);
            Assert.Equal(libraryPath, ((StubNativeLibrary)loadedLLamaNativeLibrary!).LibraryPath);
            Assert.Equal(libraryPath, ((StubNativeLibrary)loadedMtmdNativeLibrary!).LibraryPath);
        }

        private static NativeLibraryConfig CreateConfig(NativeLibraryName nativeLibraryName)
        {
            var config = (NativeLibraryConfig?)Activator.CreateInstance(
                typeof(NativeLibraryConfig),
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args: new object[] { nativeLibraryName },
                culture: null);

            return Assert.IsType<NativeLibraryConfig>(config);
        }

        private static string FindLoadableNativeLibraryPath()
        {
            var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            var extension = OperatingSystem.IsWindows()
                ? ".dll"
                : OperatingSystem.IsMacOS()
                    ? ".dylib"
                    : ".so";

            var candidateNames = OperatingSystem.IsWindows()
                ? new[] { "coreclr.dll", "clrjit.dll", "hostpolicy.dll" }
                : OperatingSystem.IsMacOS()
                    ? new[] { "libcoreclr.dylib", "libclrjit.dylib", "libhostpolicy.dylib" }
                    : new[] { "libcoreclr.so", "libclrjit.so", "libhostpolicy.so" };

            foreach (var candidateName in candidateNames)
            {
                var candidatePath = Path.Combine(runtimeDirectory, candidateName);
                if (CanLoad(candidatePath))
                {
                    return candidatePath;
                }
            }

            foreach (var candidatePath in Directory.EnumerateFiles(runtimeDirectory, $"*{extension}", SearchOption.TopDirectoryOnly))
            {
                if (CanLoad(candidatePath))
                {
                    return candidatePath;
                }
            }

            throw new InvalidOperationException($"Could not find a loadable native library in '{runtimeDirectory}'.");
        }

        private static bool CanLoad(string candidatePath)
        {
            if (!File.Exists(candidatePath))
            {
                return false;
            }

            if (!NativeLibrary.TryLoad(candidatePath, out var handle))
            {
                return false;
            }

            NativeLibrary.Free(handle);
            return true;
        }

        private sealed class StubSelectingPolicy(StubNativeLibrary library) : INativeLibrarySelectingPolicy
        {
            public IEnumerable<INativeLibrary> Apply(
                NativeLibraryConfig.Description description,
                SystemInfo systemInfo,
                NativeLogConfig.LLamaLogCallback? logCallback = null)
            {
                return new[] { library };
            }
        }

        private sealed class StubNativeLibrary(string libraryPath) : INativeLibrary
        {
            public string LibraryPath { get; } = libraryPath;

            public NativeLibraryMetadata? Metadata => null;

            public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
            {
                return new[] { LibraryPath };
            }
        }
    }
}
