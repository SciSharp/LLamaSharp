# LLamaSharp - .NET Binding for llama.cpp

![logo](Assets/LLamaSharpLogo.png)

[![Discord](https://img.shields.io/discord/1106946823282761851?label=Discord)](https://discord.gg/7wNVU65ZDY)
[![QQ Group](https://img.shields.io/static/v1?label=QQ&message=加入QQ群&color=brightgreen)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=sN9VVMwbWjs5L0ATpizKKxOcZdEPMrp8&authKey=RLDw41bLTrEyEgZZi%2FzT4pYk%2BwmEFgFcrhs8ZbkiVY7a4JFckzJefaYNW6Lk4yPX&noverify=0&group_code=985366726)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp?label=LLamaSharp)](https://www.nuget.org/packages/LLamaSharp)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cpu?label=LLamaSharp.Backend.Cpu)](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda11?label=LLamaSharp.Backend.Cuda11)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda11)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda12?label=LLamaSharp.Backend.Cuda12)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda12)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.MacMetal?label=LLamaSharp.Backend.MacMetal)](https://www.nuget.org/packages/LLamaSharp.Backend.MacMetal)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.semantic-kernel?label=LLamaSharp.semantic-kernel)](https://www.nuget.org/packages/LLamaSharp.semantic-kernel)


**The C#/.NET binding of [llama.cpp](https://github.com/ggerganov/llama.cpp). It provides APIs to inference the LLaMa Models and deploy it on local environment. It works on 
both Windows, Linux and MAC without requirment for compiling llama.cpp yourself. Its performance is close to llama.cpp.**

**Furthermore, it provides integrations with other projects such as [BotSharp](https://github.com/SciSharp/BotSharp) to provide higher-level applications and UI.**


## Documentation

- [Quick start](https://scisharp.github.io/LLamaSharp/0.5/GetStarted/)
- [Tricks for FAQ](https://scisharp.github.io/LLamaSharp/0.5/Tricks/)
- [Full documentation](https://scisharp.github.io/LLamaSharp/0.5/)
- [API reference](https://scisharp.github.io/LLamaSharp/0.5/xmldocs/)
- [Examples](./LLama.Examples/NewVersion/)

## Installation

Firstly, search `LLamaSharp` in nuget package manager and install it.

```
PM> Install-Package LLamaSharp
```

Then, search and install one of the following backends:

```
LLamaSharp.Backend.Cpu  # cpu for windows, linux and mac (mac metal is also supported)
LLamaSharp.Backend.Cuda11  # cuda11 for windows and linux
LLamaSharp.Backend.Cuda12  # cuda12 for windows and linux
LLamaSharp.Backend.MacMetal  # special for using mac metal
```

If you would like to use it with [microsoft semantic-kernel](https://github.com/microsoft/semantic-kernel), please search and install the following package:

```
LLamaSharp.semantic-kernel
```

Here's the mapping of them and corresponding model samples provided by `LLamaSharp`. If you're not sure which model is available for a version, please try our sample model.

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

Many hands make light work. If you have found any other model resource that could work for a version, we'll appreciate it for opening an PR about it! 😊

We publish the backend with cpu, cuda11 and cuda12 because they are the most popular ones. If none of them matches, please compile the [llama.cpp](https://github.com/ggerganov/llama.cpp)
from source and put the `libllama` under your project's output path ([guide](https://scisharp.github.io/LLamaSharp/0.5/ContributingGuide/)).

## FAQ

1. GPU out of memory: Please try setting `n_gpu_layers` to a smaller number.
2. Unsupported model: `llama.cpp` is under quick development and often has break changes. Please check the release date of the model and find a suitable version of LLamaSharp to install, or use the model we provide [on huggingface](https://huggingface.co/AsakusaRinne/LLamaSharpSamples).



## Usages

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

We provide the integration of ASP.NET core [here](./LLama.WebAPI). Since currently the API is not stable, please clone the repo and use it. In the future we'll publish it on NuGet.

Since we are in short of hands, if you're familiar with ASP.NET core, we'll appreciate it if you would like to help upgrading the Web API integration.

## Demo

![demo-console](Assets/console_demo.gif)

## Roadmap

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

🔳 Local document search

🔳 MAUI Integration

## Assets

Some extra model resources could be found below:

- [Qunatized models provided by LLamaSharp Authors](https://huggingface.co/AsakusaRinne/LLamaSharpSamples)
- [eachadea/ggml-vicuna-13b-1.1](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main)
- [TheBloke/wizardLM-7B-GGML](https://huggingface.co/TheBloke/wizardLM-7B-GGML)
- Magnet: [magnet:?xt=urn:btih:b8287ebfa04f879b048d4d4404108cf3e8014352&dn=LLaMA](magnet:?xt=urn:btih:b8287ebfa04f879b048d4d4404108cf3e8014352&dn=LLaMA)

The weights included in the magnet is exactly the weights from [Facebook LLaMa](https://github.com/facebookresearch/llama).

The prompts could be found below:

- [llama.cpp prompts](https://github.com/ggerganov/llama.cpp/tree/master/prompts) 
- [ChatGPT_DAN](https://github.com/0xk1h0/ChatGPT_DAN)
- [awesome-chatgpt-prompts](https://github.com/f/awesome-chatgpt-prompts)
- [awesome-chatgpt-prompts-zh](https://github.com/PlexPt/awesome-chatgpt-prompts-zh) (Chinese)

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
