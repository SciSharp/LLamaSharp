![logo](Assets/LLamaSharpLogo.png)

[![Discord](https://img.shields.io/discord/1106946823282761851?label=Discord)](https://discord.gg/7wNVU65ZDY)
[![QQ Group](https://img.shields.io/static/v1?label=QQ&message=加入QQ群&color=brightgreen)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=sN9VVMwbWjs5L0ATpizKKxOcZdEPMrp8&authKey=RLDw41bLTrEyEgZZi%2FzT4pYk%2BwmEFgFcrhs8ZbkiVY7a4JFckzJefaYNW6Lk4yPX&noverify=0&group_code=985366726)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp?label=LLamaSharp)](https://www.nuget.org/packages/LLamaSharp)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cpu?label=LLamaSharp.Backend.Cpu)](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda11?label=LLamaSharp.Backend.Cuda11)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda11)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda12?label=LLamaSharp.Backend.Cuda12)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda12)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.MacMetal?label=LLamaSharp.Backend.MacMetal)](https://www.nuget.org/packages/LLamaSharp.Backend.MacMetal)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.semantic-kernel?label=LLamaSharp.semantic-kernel)](https://www.nuget.org/packages/LLamaSharp.semantic-kernel)


**The C#/.NET binding of [llama.cpp](https://github.com/ggerganov/llama.cpp). It provides higher-level APIs to inference the LLaMA Models and deploy it on local device with C#/.NET. It works on 
both Windows, Linux and MAC without requirment for compiling llama.cpp yourself. Even without GPU or not enought GPU memory, you can still apply LLaMA models well with this repo. 🤗**

**Furthermore, it provides integrations with other projects such as [semantic-kernel](https://github.com/microsoft/semantic-kernel), [kernel-memory](https://github.com/microsoft/kernel-memory) and [BotSharp](https://github.com/SciSharp/BotSharp) to provide higher-level applications.**


## Documentation

- [Quick start](https://scisharp.github.io/LLamaSharp/0.5/GetStarted/)
- [Tricks for FAQ](https://scisharp.github.io/LLamaSharp/0.5/Tricks/)
- [Full documentation](https://scisharp.github.io/LLamaSharp/0.5/)
- [API reference](https://scisharp.github.io/LLamaSharp/0.5/xmldocs/)

## Examples
- [Official Console Examples](./LLama.Examples/NewVersion/)
- [Unity Demo](https://github.com/eublefar/LLAMASharpUnityDemo)
- [LLamaStack (with WPF and Web support)](https://github.com/saddam213/LLamaStack)


## Installation

Firstly, search `LLamaSharp` in nuget package manager and install it.

```
PM> Install-Package LLamaSharp
```

Then, search and install one of the following backends. (Please don't install two or more)

```
LLamaSharp.Backend.Cpu  # cpu for windows, linux and mac (mac metal is also supported)
LLamaSharp.Backend.Cuda11  # cuda11 for windows and linux
LLamaSharp.Backend.Cuda12  # cuda12 for windows and linux
LLamaSharp.Backend.MacMetal  # Removed after v0.8.0, metal support has been moved to cpu version now
```

We publish these backends because they are the most popular ones. If none of them matches, please compile the [llama.cpp](https://github.com/ggerganov/llama.cpp) yourself. In this case, please **DO NOT** install the backend packages, instead, add your DLL to your project and ensure it will be copied to the output directory when compiling your project. For more informations please refer to ([this guide](https://scisharp.github.io/LLamaSharp/0.5/ContributingGuide/)).

For [microsoft semantic-kernel](https://github.com/microsoft/semantic-kernel) integration, please search and install the following package:

```
LLamaSharp.semantic-kernel
```

For [microsoft kernel-memory](https://github.com/microsoft/kernel-memory) integration, please search and install the following package (currently kernel-memory only supports net6.0):

```
LLamaSharp.kernel-memory
```

### Tips for choosing a version

In general, there may be some break changes between two minor releases, for example 0.5.1 and 0.6.0. On the contrary, we don't introduce API break changes in patch release. Therefore it's recommended to keep the highest patch version of a minor release. For example, keep 0.5.6 instead of 0.5.3.


### Mapping from LLamaSharp to llama.cpp
Here's the mapping of them and corresponding model samples provided by `LLamaSharp`. If you're not sure which model is available for a version, please try our sample model.

The llama.cpp commit id will help if you want to compile a DLL yourself.

| LLamaSharp.Backend | LLamaSharp | Verified Model Resources | llama.cpp commit id |
| - | - | -- | - |
| - | v0.2.0 | This version is not recommended to use. | - |
| - | v0.2.1 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama), [Vicuna (filenames with "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | - |
| v0.2.2 | v0.2.2, v0.2.3 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama_ggmlv2), [Vicuna (filenames without "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | 63d2046 |
| v0.3.0, v0.3.1 | v0.3.0, v0.4.0 | [LLamaSharpSamples v0.3.0](https://huggingface.co/AsakusaRinne/LLamaSharpSamples/tree/v0.3.0), [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/main) | 7e4ea5b |
| v0.4.1-preview (cpu only) | v0.4.1-preview | [Open llama 3b](https://huggingface.co/SlyEcho/open_llama_3b_ggml), [Open Buddy](https://huggingface.co/OpenBuddy/openbuddy-llama-ggml)| aacdbd4 |
| v0.4.2-preview (cpu,cuda11) |v0.4.2-preview | [Llama2 7b GGML](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGML)| 3323112 |
| v0.5.1 | v0.5.1 | [Llama2 7b GGUF](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF)| 6b73ef1 |
| v0.6.0 | v0.6.0 | | [cb33f43](https://github.com/ggerganov/llama.cpp/commit/cb33f43a2a9f5a5a5f8d290dd97c625d9ba97a2f) |
| v0.7.0 | v0.7.0 | [Thespis-13B](https://huggingface.co/TheBloke/Thespis-13B-v0.5-GGUF/tree/main?not-for-all-audiences=true), [LLaMA2-7B](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF) | [207b519](https://github.com/ggerganov/llama.cpp/commit/207b51900e15cc7f89763a3bb1c565fe11cbb45d) |

Many hands make light work. If you have found any other model resource that could work for a version, we'll appreciate it for opening an PR about it! 😊


## FAQ

1. GPU out of memory: Please try setting `n_gpu_layers` to a smaller number.
2. Unsupported model: `llama.cpp` is under quick development and often has break changes. Please check the release date of the model and find a suitable version of LLamaSharp to install, or generate `gguf` format weights from original weights yourself.
3. Cannot load native lirary: 1) ensure you installed one of the backend packages. 2) Run `NativeLibraryConfig.WithLogs()` at the very beginning of your code to print more informations. 3) check if your system supports avx2, which is the default settings of official runtimes now. If not, please compile llama.cpp yourself and specify it with `NativeLibraryConfig.WithLibrary`.



## Quick Start

#### Model Inference and Chat Session

LLamaSharp provides two ways to run inference: `LLamaExecutor` and `ChatSession`. The chat session is a higher-level wrapping of the executor and the model. Here's a simple example to use chat session.

```cs
using LLama.Common;
using LLama;

string modelPath = "<Your model path>"; // change it to your own model path
var prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:"; // use the "chat-with-bob" prompt here.

// Load a model
var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024,
    Seed = 1337,
    GpuLayerCount = 5
};
using var model = LLamaWeights.LoadFromFile(parameters);

// Initialize a chat session
using var context = model.CreateContext(parameters);
var ex = new InteractiveExecutor(context);
ChatSession session = new ChatSession(ex);

// show the prompt
Console.WriteLine();
Console.Write(prompt);

// run the inference in a loop to chat with LLM
while (prompt != "stop")
{
    foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
    {
        Console.Write(text);
    }
    prompt = Console.ReadLine();
}

// save the session
session.SaveSession("SavedSessionPath");
```

#### Quantization

The following example shows how to quantize the model. With LLamaSharp you needn't to compile c++ project and run scripts to quantize the model, instead, just run it in C#.

```cs
string srcFilename = "<Your source path>";
string dstFilename = "<Your destination path>";
string ftype = "q4_0";
if(Quantizer.Quantize(srcFileName, dstFilename, ftype))
{
    Console.WriteLine("Quantization succeed!");
}
else
{
    Console.WriteLine("Quantization failed!");
}
```

For more usages, please refer to [Examples](./LLama.Examples).

#### Web API

We provide [the integration of ASP.NET core](./LLama.WebAPI) and a [web app demo](./LLama.Web). Please clone the repo to have a try.

Since we are in short of hands, if you're familiar with ASP.NET core, we'll appreciate it if you would like to help upgrading the Web API integration.

## Console Demo

![demo-console](Assets/console_demo.gif)

## How to Find a Model

Models in format `gguf` are valid for LLamaSharp (and `ggml` before v0.5.1). If you're new to LLM/LLaMA, it's a good choice to search `LLama` and `gguf` on [huggingface](https://huggingface.co/) to find a model.

Another choice is generate gguf format file yourself with a pytorch weight (or any other), pleae refer to [convert.py](https://github.com/ggerganov/llama.cpp/blob/master/convert.py) and [convert-llama-ggml-to-gguf.py](https://github.com/ggerganov/llama.cpp/blob/master/convert-llama-ggml-to-gguf.py) to get gguf file through a ggml transformation.

## Features

---

✅: completed. ⚠️: outdated for latest release but will be updated. 🔳: not completed

---

✅ LLaMa model inference

✅ Embeddings generation, tokenization and detokenization

✅ Chat session

✅ Quantization

✅ Grammar

✅ State saving and loading

⚠️ BotSharp Integration

✅ ASP.NET core Integration

✅ Semantic-kernel Integration

🔳 Fine-tune

✅ Local document search (enabled by kernel-memory now)

🔳 MAUI Integration

## Contributing

Any contribution is welcomed! Please read the [contributing guide](https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/). You can do one of the followings to help us make `LLamaSharp` better:

- Append a model link that is available for a version. (This is very important!)
- Star and share `LLamaSharp` to let others know it.
- Add a feature or fix a BUG.
- Help to develop Web API and UI integration.
- Just start an issue about the problem you met!

## Contact us

Join our chat on [Discord](https://discord.gg/7wNVU65ZDY).

Join [QQ group](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=sN9VVMwbWjs5L0ATpizKKxOcZdEPMrp8&authKey=RLDw41bLTrEyEgZZi%2FzT4pYk%2BwmEFgFcrhs8ZbkiVY7a4JFckzJefaYNW6Lk4yPX&noverify=0&group_code=985366726)

## License

This project is licensed under the terms of the MIT license.
