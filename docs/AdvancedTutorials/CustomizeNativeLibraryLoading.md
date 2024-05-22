# Customize the native library loading process

In [the tutorial of native library config](../Tutorials/NativeLibraryConfig.md), we introduces how to auto-select a best native library as backend with your configurations. In this tutorial, we're taking a step further, to customize the native library loading process in a much more flexible way.

## Customize the policy of the selection

`INativeLibrarySelectingPolicy` is responsible for selecting the native libraries to load and sort them in order of priority. You could implement this interface and register your implementation by calling `NativeLibraryConfig.WithSelectingPolicy`.

Note that when you're using your own implementation, the native libraries returned from `INativeLibrarySelectingPolicy.Apply` are all the libraries that are possibly used. It might spend lots of time of you to complete a robust implementation, but you can always fallback to the default behavior by calling `DefaultNativeLibrarySelectingPolicy` when no suitable library is found.

```cs
public interface INativeLibrarySelectingPolicy
{
    /// <summary>
    /// Select the native library.
    /// </summary>
    /// <param name="description">The description of the user's configuration.</param>
    /// <param name="systemInfo">The system information of the current machine.</param>
    /// <param name="logCallback">The log callback.</param>
    /// <returns>The information of the selected native library files, in order by priority from the beginning to the end.</returns>
    IEnumerable<INativeLibrary> Apply(NativeLibraryConfig.Description description, SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null);
}
```

## Customize the native library

The native library in LLamaSharp is not just a file path. Instead, it's abstracted to `INativeLibrary`, which contains the information of it and allows finer-grained control of the loading process.

```cs
public interface INativeLibrary
{
    /// <summary>
    /// Metadata of this library.
    /// </summary>
    NativeLibraryMetadata? Metadata { get; }

    /// <summary>
    /// Prepare the native library file and returns the local path of it.
    /// If it's a relative path, LLamaSharp will search the path in the search directies you set.
    /// </summary>
    /// <param name="systemInfo">The system information of the current machine.</param>
    /// <param name="logCallback">The log callback.</param>
    /// <returns>
    /// The relative paths of the library. You could return multiple paths to try them one by one. If no file is available, please return an empty array.
    /// </returns>
    IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null);
}
```

`INativeLibrary.Metadata` contains the information of the native library. When you implement the interface, you can ignore it and return `null`. However, the best practice will always include implemenbting this peoperty because it will provide much more information when there's an anexpected behavior.

`INativeLibrary.Prepare` is a method which allows you to do some preparations before loading the library. For example, you can move the file, download the file, output some logs, etc. It is supposed to return a list of file paths in order of priority. 

There are several implementations inside LLamaSharp, which are `NativeLibraryFromPath`, `NativeLibraryWithAvx`, `NativeLibraryWithCuda` and `NativeLibraryWithMacOrFallback`. Please refer to their implementations if you find it difficult to implement `INativeLibrary`.

## How does the loading work

Here're the steps for the native library loading in LLamaSharp.

1. Gather and validate the user's configuration for native library loading. (`NativeLibraryConfig`)
2. Gather the system information, mainly including platform and device.
3. Apply the selecting policy to get the list of native libraries to load.
4. For each of the selected native library, call the `INativeLibrary.Prepare` method to get the file paths. Then Try to load the library from the paths in turn. If the library is loaded successfully with any path of them, the loop will be broken.

## Example: use remote library files

Though the native library downloading feature will be introduced as discussed in https://github.com/SciSharp/LLamaSharp/issues/670, it might have some security concerns. Here's an example to implement the remote native library downloading.

Firstly, the native library with downloading process should be implemented.

```cs
public class AutoDownloadedLibraries
{
    // Wrap a cuda native library
    public class Cuda: INativeLibrary
    {
        // the default cuda native library implementation in LLamaSHarp
        private NativeLibraryWithCuda _cudaLibrary;
        // Some download settings
        private NativeLibraryDownloadSettings _settings;

        public Cuda(NativeLibraryWithCuda cudaLibrary, NativeLibraryDownloadSettings settings)
        {
            _cudaLibrary = cudaLibrary;
            _settings = settings;
        }

        public NativeLibraryMetadata? Metadata => _cudaLibrary.Metadata;

        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
        {
            foreach(var relativePath in _cudaLibrary.Prepare(systemInfo, logCallback))
            {
                // try to use the default path first. If loaded successfully, the download will not be triggered.
                yield return relativePath;
                // download the file.
                // NOTE: be sure to complete the downloading process here. You CANNOT make `Prepare` as an async method.
                var path = NativeLibraryDownloader.DownloadLibraryFile(_settings, relativePath, logCallback).Result;
                // if the downloading is successful, return the path of the downloaded file.
                if (path is not null)
                {
                    yield return path;
                }
            }
        }
    }
}
```

Then, implement the selecting policy for the native libraries above

```cs
public class SelectingPolicyWithAutoDownload: INativeLibrarySelectingPolicy
{
    // making this class a wrapper for the default policy.
    private DefaultNativeLibrarySelectingPolicy _defaultPolicy = new();
    // record the download settings.
    private NativeLibraryDownloadSettings _downloadSettings;

    internal SelectingPolicyWithAutoDownload(NativeLibraryDownloadSettings downloadSettings)
    {
        _downloadSettings = downloadSettings;
    }

    public IEnumerable<INativeLibrary> Apply(NativeLibraryConfig.Description description, SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
    {
        foreach(var library in _defaultPolicy.Apply(description, systemInfo, logCallback))
        {
            // check the return type and returns the corresponding wrapper
            if(library is NativeLibraryWithCuda cudaLibrary)
            {
                yield return new AutoDownloadedLibraries.Cuda(cudaLibrary, _downloadSettings);
            }
            else if(library is NativeLibraryWithAvx avxLibrary)
            {
                yield return new AutoDownloadedLibraries.Avx(avxLibrary, _downloadSettings);
            }
            else if(library is NativeLibraryWithMacOrFallback macLibrary)
            {
                yield return new AutoDownloadedLibraries.MacOrFallback(macLibrary, _downloadSettings);
            }
            // Generally, you don't need to download the DLL if the user specify a path.
            // But if you want, you can certainly add a wrapper for it.
            else if(library is NativeLibraryFromPath)
            {
                yield return library;
            }
            // It's also reasonable to throw an exception here.
            else
            {
                yield return library;
            }
        }
    }
}
```

Finally, add an extension to make an API for users to enable this feature.

```cs
 public static class NativeLibraryAutoDownloadExtension
{
    public static NativeLibraryConfig WithAutoDownload(this NativeLibraryConfig config, bool enable = true, NativeLibraryDownloadSettings? settings = null)
    {
        if (config.LibraryHasLoaded)
        {
            throw new Exception("The library has already loaded, you can't change the configurations. " +
                "Please finish the configuration setting before any call to LLamaSharp native APIs." +
                "Please use NativeLibraryConfig.DryRun if you want to see whether it's loaded successfully " +
                "but still have chance to modify the configurations.");
        }
        if (enable)
        {
            if(settings is null)
            {
                settings = NativeLibraryDownloadSettings.Create();
            }
            // If you want to return an relative path in `INativeLibrary.Prepare`, 
            // be sure to add the downloading directory to the search directories.
            var defaultLocalDir = NativeLibraryDownloadSettings.GetDefaultLocalDir(settings);

            // When using auto-download, this should be the only search this directory.
            List<string> searchDirectoriesForDownload = [settings.LocalDir!];
            // unless extra search paths are added by the user.
            searchDirectoriesForDownload.AddRange(settings.ExtraSearchDirectories ?? []);
            config.WithSearchDirectories(searchDirectoriesForDownload);

            // register you selecting policy.
            config.WithSelectingPolicy(new SelectingPolicyWithAutoDownload(settings));
        }
        return config;
    }
}
```

Now your users are now able to enable this feature by calling `NativeLibraryConfig.All.WithAutoDownload`!