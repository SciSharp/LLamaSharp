# Customize the native library loading

As indicated in [Architecture](../Architecture.md), LLamaSharp uses the native library to run the LLM models. Sometimes you may want to compile the native library yourself, or just dynamically load the library due to the environment of your user of your application. Luckily, since version 0.7.0, dynamic loading of native library has been supported! That allows you to customize the native library loading process.


## When you should compile the native library yourself

Before introducing the way to customize native library loading, please follow the tips below to see if you need to compile the native library yourself, rather than use the published backend packages, which contain native library files for multiple targets.

1. Your device/environment has not been supported by any published backend packages. For example, vulkan has not been supported yet. In this case, it will mean a lot to open an issue to tell us you are using it. Since our support for new backend will have a delay, you could compile yourself before that.
2. You want to gain the best performance of LLamaSharp. Because LLamaSharp offloads the model to both GPU and CPU, the performance is significantly related with CPU if your GPU memory size is small. AVX ([Advanced Vector Extensions](https://en.wikipedia.org/wiki/Advanced_Vector_Extensions)) and BLAS ([Basic Linear Algebra Subprograms](https://en.wikipedia.org/wiki/Basic_Linear_Algebra_Subprograms)) are the most important ways to accelerate the CPU computation. By default, LLamaSharp disables the support for BLAS and use AVX2 for CUDA backend yet. If you would like to enable BLAS or use AVX 512 along with CUDA, please compile the native library yourself, following the [instructions here](../ContributingGuide.md).
3. You want to debug the c++ code.


## Use NativeLibraryConfig

We provide `LLama.Native.NativeLibraryConfig` class with singleton mode to allow users to customize the loading process of the native library. Any method of it should be called before the model loading, because a native library file must be decided before any model is loaded.

### Load specified native library file

All you need to do is adding the following code to the very beginning of your code.

```cs
NativeLibraryConfig.Instance.WithLibrary("<Your native library path>");
```

### Automatically select one from multiple native library files

Let's consider this case: you don't know your user's device when distributing your application, so you put all the possible native libraries in a folder and want to select the best one depending on the user's device. LLamaSharp allows you to define the strategy to do it.

- `NativeLibraryConfig.Instance.WithCuda`: decide if you want to use cuda if possible.
- `NativeLibraryConfig.Instance.WithAvx`: decide the highest AVX level you want to use if possible.
- `NativeLibraryConfig.Instance.WithSearchDirectory`: specify the directory to search the native library files.
- `NativeLibraryConfig.Instance.WithAutoFallback`: whether to allow fall back to other options if no native library that matches your specified settings could be found.

### Set the log level of native library loading

```cs
NativeLibraryConfig.Instance.WithLogs();
```

There are four log levels, which are error, warning, info and debug. If you are not sure if the correct library is selected, please set log level to `info` to see the full logs.
