# LLamaSharp Contributing Guide

Hi, welcome to develop LLamaSharp with us together! We are always open for every contributor and any format of contributions! If you want to maintain this library actively together, please contact us to get the write access after some PRs. (Email: AsakusaRinne@gmail.com)

In this page, we introduce how to make contributions here easily. 😊

## The goal of LLamaSharp

At the beginning, LLamaSharp is a C# binding of [llama.cpp](https://github.com/ggerganov/llama.cpp). It provided only some wrappers for llama.cpp to let C#/.NET users could run LLM models on their local device efficiently even if without any experience with C++. After around a year of development, more tools and integrations has been added to LLamaSharp, significantly expanding the application of LLamaSharp. Though llama.cpp is still the only backend of LLamaSharp, the goal of this repository is more likely to be an efficient and easy-to-use library of LLM inference, rather than just a binding of llama.cpp.

In this way, our development of LLamaSharp is divided into two main directions:

1. To make LLamaSharp more efficient. For example, `BatchedExecutor` could accept multiple queries and generate the response for them at the same time, which significantly improves the throughput. This part is always related with native APIs and executors in LLamaSharp.
2. To make it easier to use LLamaSharp. We believe the best library is to let users build powerful functionalities with simple code. Higher-level APIs and integrations with other libraries are the key points of it.


## How to compile the native library from source

If you want to contribute to the first direction of our goal, you may need to compile the native library yourself.

Firstly, please follow the instructions in [llama.cpp readme](https://github.com/ggerganov/llama.cpp#build) to configure your local environment. Most importantly, CMake with version higher than 3.14 should be installed on your device.

Secondly, clone the llama.cpp repositories. You could manually clone it and checkout to the right commit according to [Map of LLamaSharp and llama.cpp versions](https://github.com/SciSharp/LLamaSharp?tab=readme-ov-file#map-of-llamasharp-and-llama.cpp-versions), or use clone the submodule of LLamaSharp when cloning LLamaSharp.

```shell
git clone --recursive https://github.com/SciSharp/LLamaSharp.git
```

If you want to support cublas in the compilation, please make sure that you've installed it. If you are using Intel CPU, please check the highest AVX ([Advanced Vector Extensions](https://en.wikipedia.org/wiki/Advanced_Vector_Extensions)) level that is supported by your device.

As shown in [llama.cpp cmake file](https://github.com/ggerganov/llama.cpp/blob/master/CMakeLists.txt), there are many options that could be enabled or disabled when building the library. The following ones are commonly used when using it as a native library of LLamaSharp.

```cpp
option(BUILD_SHARED_LIBS                "build shared libraries") // Please always enable it 
option(LLAMA_NATIVE                     "llama: enable -march=native flag") // Could be disabled
option(LLAMA_AVX                        "llama: enable AVX") // Enable it if the highest supported avx level is AVX
option(LLAMA_AVX2                       "llama: enable AVX2") // Enable it if the highest supported avx level is AVX2
option(LLAMA_AVX512                     "llama: enable AVX512") // Enable it if the highest supported avx level is AVX512
option(LLAMA_BLAS                       "llama: use BLAS") // Enable it if you want to use BLAS library to accelerate the computation on CPU
option(LLAMA_CUDA                       "llama: use CUDA") // Enable it if you have CUDA device
option(LLAMA_CLBLAST                    "llama: use CLBlast") // Enable it if you have a device with CLBLast or OpenCL support, for example, some AMD GPUs.
option(LLAMA_VULKAN                     "llama: use Vulkan") // Enable it if you have a device with Vulkan support
option(LLAMA_METAL                      "llama: use Metal") // Enable it if you are using a MAC with Metal device.
option(LLAMA_BUILD_TESTS                "llama: build tests") // Please disable it.
option(LLAMA_BUILD_EXAMPLES             "llama: build examples") // Please disable it.
option(LLAMA_BUILD_SERVER               "llama: build server example")// Please disable it.
```

Most importantly, `-DBUILD_SHARED_LIBS=ON` must be added to the cmake instruction and other options depends on you. For example, when building with cublas but without openblas, use the following instruction:

```bash
mkdir build && cd build
cmake .. -DLLAMA_CUBLAS=ON -DBUILD_SHARED_LIBS=ON
cmake --build . --config Release
```

Now you could find the `llama.dll`, `libllama.so` or `llama.dylib` in your build directory (or `build/bin`). 

To load the compiled native library, please add the following code to the very beginning of your code.

```cs
NativeLibraryConfig.Instance.WithLibrary("<Your native library path>");
```


## Add a new feature to LLamaSharp

After refactoring the framework in `v0.4.0`, LLamaSharp will try to maintain the backward compatibility. However, in the following cases a breaking change will be required:

1. Due to some break changes in [llama.cpp](https://github.com/ggerganov/llama.cpp), making a breaking change will help to maintain the good abstraction and friendly user APIs.
2. An important feature cannot be implemented unless refactoring some parts.
3. After some discussions, an agreement was reached that making the break change is reasonable.

If a new feature could be added without introducing any break change, please **open a PR** rather than open an issue first. We will never refuse the PR but help to improve it, unless it's malicious.

When adding the feature, please take care of the namespace and the naming convention. For example, if you are adding an integration for WPF, please put the code under namespace `LLama.WPF` or `LLama.Integration.WPF` instead of putting it under the root namespace. The naming convention of LLamaSharp follows the pascal naming convention, but in some parts that are invisible to users, you can do whatever you want.

## Find the problem and fix the BUG

If the issue is related to the LLM internal behaviour, such as endless generating the response, the best way to find the problem is to do comparison test between llama.cpp and LLamaSharp.

You could use exactly the same prompt, the same model and the same parameters to run the inference in llama.cpp and LLamaSharp respectively to see if it's really a problem caused by the implementation in LLamaSharp.

If the experiment showed that it worked well in llama.cpp but didn't in LLamaSharp, a search for the problem could be started. While the reason of the problem could be various, the best way I think is to add log-print in the code of llama.cpp and use it in LLamaSharp after compilation. Thus, when running LLamaSharp, you could see what happened in the native library.

During the BUG fix process, please don't hesitate to discuss together when you are blocked.

## Add integrations

All kinds of integration are welcomed here! Currently the following integrations have been added but still need improvement:

1. semantic-kernel
2. kernel-memory
3. BotSharp (maintained in SciSharp/BotSharp repo)
4. Langchain (maintained in tryAGI/LangChain repo)

If you find another library that is good to be integrated, please open an issue to let us know!


## Add examples

There're mainly two ways to add an example:

1. Add the example to `LLama.Examples` of the repository.
2. Put the example in another repository and add the link to the readme or docs of LLamaSharp.

## Add documents

LLamaSharp uses [mkdocs](https://github.com/mkdocs/mkdocs) to build the documentation, please follow the tutorial of mkdocs to add or modify documents in LLamaSharp.
