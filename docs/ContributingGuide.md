# LLamaSharp Contributing Guide

Hi, welcome to develop LLamaSharp with us together! We are always open for every contributor and any format of contributions! If you want to maintain this library actively together, please contact us to get the write access after some PRs. (Email: AsakusaRinne@gmail.com)

In this page, we'd like to introduce how to make contributions here easily. 😊

## Compile the native library from source

Firstly, please clone the [llama.cpp](https://github.com/ggerganov/llama.cpp) repository and following the instructions in [llama.cpp readme](https://github.com/ggerganov/llama.cpp#build) to configure your local environment.

If you want to support cublas in the compilation, please make sure that you've installed the cuda.

When building from source, please add `-DBUILD_SHARED_LIBS=ON` to the cmake instruction. For example, when building with cublas but without openblas, use the following instruction:

```bash
cmake .. -DLLAMA_CUBLAS=ON -DBUILD_SHARED_LIBS=ON
```

After running `cmake --build . --config Release`, you could find the `llama.dll`, `llama.so` or `llama.dylib` in your build directory. After pasting it to `LLamaSharp/LLama/runtimes` , you can use it as the native library in LLamaSharp.


## Add a new feature to LLamaSharp

After refactoring the framework in `v0.4.0`, LLamaSharp will try to maintain the backward compatibility. However, in the following cases a breaking change will be required:

1. Due to some break changes in [llama.cpp](https://github.com/ggerganov/llama.cpp), making a breaking change will help to maintain the good abstraction and friendly user APIs.
2. A very important feature cannot be implemented unless refactoring some parts.
3. After some discussions, an agreement was reached that making the break change is reasonable.

If a new feature could be added without introducing any break change, please **open a PR** rather than open an issue first. We will never refuse the PR but help to improve it, unless it's malicious.

When adding the feature, please take care of the namespace and the naming convention. For example, if you are adding an integration for WPF, please put the code under namespace `LLama.WPF` or `LLama.Integration.WPF` instead of putting it under the root namespace. The naming convention of LLamaSharp follows the pascal naming convention, but in some parts that are invisible to users, you can do whatever you want.

## Find the problem and fix the BUG

If the issue is related to the LLM internal behaviour, such as endless generating the response, the best way to find the problem is to do comparison test between llama.cpp and LLamaSharp.

You could use exactly the same prompt, the same model and the same parameters to run the inference in llama.cpp and LLamaSharp respectively to see if it's really a problem caused by the implementation in LLamaSharp.

If the experiment showed that it worked well in llama.cpp but didn't in LLamaSharp, a search for the problem could be started. While the reason of the problem could be various, the best way I think is to add log-print in the code of llama.cpp and use it in LLamaSharp after compilation. Thus, when running LLamaSharp, you could see what happened in the native library.

After finding out the reason, a painful but happy process comes. When working on the BUG fix, there's only one rule to follow, that is keeping the examples working well. If the modification fixed the BUG but impact on other functions, it would not be a good fix.

During the BUG fix process, please don't hesitate to discuss together when you stuck on something.

## Add integrations

All kinds of integration are welcomed here! Currently the following integrations are under work or on our schedule:

1. BotSharp
2. semantic-kernel
3. Unity

Besides, for some other integrations, like `ASP.NET core`, `SQL`, `Blazor` and so on, we'll appreciate it if you could help with that. If the time is limited for you, providing an example for it also means a lot!

## Add examples

There're mainly two ways to add an example:

1. Add the example to `LLama.Examples` of the repository.
2. Put the example in another repository and add the link to the readme or docs of LLamaSharp.

## Add documents

LLamaSharp uses [mkdocs](https://github.com/mkdocs/mkdocs) to build the documentation, please follow the tutorial of mkdocs to add or modify documents in LLamaSharp.