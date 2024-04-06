![logo](Assets/LLamaSharpLogo.png)

[![Discord](https://img.shields.io/discord/1106946823282761851?label=Discord)](https://discord.gg/7wNVU65ZDY)
[![QQ Group](https://img.shields.io/static/v1?label=QQ&message=加入QQ群&color=brightgreen)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=sN9VVMwbWjs5L0ATpizKKxOcZdEPMrp8&authKey=RLDw41bLTrEyEgZZi%2FzT4pYk%2BwmEFgFcrhs8ZbkiVY7a4JFckzJefaYNW6Lk4yPX&noverify=0&group_code=985366726)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp?label=LLamaSharp)](https://www.nuget.org/packages/LLamaSharp)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cpu?label=LLamaSharp.Backend.Cpu)](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda11?label=LLamaSharp.Backend.Cuda11)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda11)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.Cuda12?label=LLamaSharp.Backend.Cuda12)](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda12)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.semantic-kernel?label=LLamaSharp.semantic-kernel)](https://www.nuget.org/packages/LLamaSharp.semantic-kernel)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.kernel-memory?label=LLamaSharp.kernel-memory)](https://www.nuget.org/packages/LLamaSharp.kernel-memory)
[![LLamaSharp Badge](https://img.shields.io/nuget/v/LLamaSharp.Backend.OpenCL?label=LLamaSharp.Backend.OpenCL)](https://www.nuget.org/packages/LLamaSharp.Backend.OpenCL)


**LLamaSharp is a cross-platform library to run 🦙LLaMA/LLaVA model (and others) on your local device. Based on [llama.cpp](https://github.com/ggerganov/llama.cpp), inference with LLamaSharp is efficient on both CPU and GPU. With the higher-level APIs and RAG support, it's convenient to deploy LLM (Large Language Model) in your application with LLamaSharp.**

**Please star the repo to show your support for this project!🤗**

---


<details>
  <summary>Table of Contents</summary>
  <ul>
    <li><a href="#Documentation">Documentation</a></li>
    <li><a href="#Console Demo">Console Demo</a></li>
    <li><a href="#Integrations & Examples">Integrations & Examples</a></li>
    <li><a href="#Get started">Get started</a></li>
    <li><a href="#FAQ">FAQ</a></li>
    <li><a href="#Contributing">Contributing</a></li>
    <li><a href="#Join the community">Join the community</a></li>
    <li><a href="#Star history">Star history</a></li>
    <li><a href="#Contributor wall of fame">Contributor wall of fame</a></li>
    <li><a href="#Map of LLamaSharp and llama.cpp versions">Map of LLamaSharp and llama.cpp versions</a></li>
  </ul>
</details>

## 📖Documentation

- [Quick start](https://scisharp.github.io/LLamaSharp/latest/QuickStart/)
- [FAQ](https://scisharp.github.io/LLamaSharp/latest/FAQ/)
- [Tutorial](https://scisharp.github.io/LLamaSharp/latest/Tutorials/NativeLibraryConfig/)
- [Full documentation](https://scisharp.github.io/LLamaSharp/latest/)
- [API reference](https://scisharp.github.io/LLamaSharp/latest/xmldocs/)


## 📌Console Demo

<table class="center">
    <tr style="line-height: 0">
    <td width=50% height=30 style="border: none; text-align: center">LLaMA</td>
    <td width=50% height=30 style="border: none; text-align: center">LLaVA</td>
    </tr>
    <tr>
    <td width=25% style="border: none"><img src="Assets/console_demo.gif" style="width:100%"></td>
    <td width=25% style="border: none"><img src="Assets/llava_demo.gif" style="width:100%"></td>
    </tr>
</table>


## 🔗Integrations & Examples

There are integarions for the following libraries, making it easier to develop your APP. Integrations for semantic-kernel and kernel-memory are developed in LLamaSharp repository, while others are developed in their own repositories.

- [semantic-kernel](https://github.com/microsoft/semantic-kernel): an SDK that integrates LLM like OpenAI, Azure OpenAI, and Hugging Face.
- [kernel-memory](https://github.com/microsoft/kernel-memory): a multi-modal AI Service specialized in the efficient indexing of datasets through custom continuous data hybrid pipelines, with support for RAG ([Retrieval Augmented Generation](https://en.wikipedia.org/wiki/Prompt_engineering#Retrieval-augmented_generation)), synthetic memory, prompt engineering, and custom semantic memory processing.
- [BotSharp](https://github.com/SciSharp/BotSharp): an open source machine learning framework for AI Bot platform builder.
- [Langchain](https://github.com/tryAGI/LangChain): a framework for developing applications powered by language models.


The following examples show how to build APPs with LLamaSharp.

- [Official Console Examples](./LLama.Examples/)
- [Unity Demo](https://github.com/eublefar/LLAMASharpUnityDemo)
- [LLamaStack (with WPF and Web demo)](https://github.com/saddam213/LLamaStack)
- [Blazor Demo (with Model Explorer)](https://github.com/alexhiggins732/BLlamaSharp.ChatGpt.Blazor)
- [ASP.NET Demo](./LLama.Web/)

![LLamaShrp-Integrations](./Assets/LLamaSharp-Integrations.png)


## 🚀Get started

### Installation

To gain high performance, LLamaSharp interacts with a native library compiled from c++, which is called `backend`. We provide backend packages for Windows, Linux and MAC with CPU, Cuda, Metal and OpenCL. You **don't** need to handle anything about c++ but just install the backend packages.

If no published backend match your device, please open an issue to let us know. If compiling c++ code is not difficult for you, you could also follow [this guide](./docs/ContributingGuide.md) to compile a backend and run LLamaSharp with it.

1.  Install [LLamaSharp](https://www.nuget.org/packages/LLamaSharp) package on NuGet:

```
PM> Install-Package LLamaSharp
```

2. Install one or more of these backends, or use self-compiled backend.

   - [`LLamaSharp.Backend.Cpu`](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu): Pure CPU for Windows & Linux & MAC. Metal (GPU) support for MAC.
   - [`LLamaSharp.Backend.Cuda11`](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda11): CUDA11 for Windows & Linux.
   - [`LLamaSharp.Backend.Cuda12`](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda12): CUDA 12 for Windows & Linux.
   - [`LLamaSharp.Backend.OpenCL`](https://www.nuget.org/packages/LLamaSharp.Backend.OpenCL): OpenCL for Windows & Linux.

3. (optional) For [Microsoft semantic-kernel](https://github.com/microsoft/semantic-kernel) integration, install the [LLamaSharp.semantic-kernel](https://www.nuget.org/packages/LLamaSharp.semantic-kernel) package.
4. (optional) To enable RAG support, install the [LLamaSharp.kernel-memory](https://www.nuget.org/packages/LLamaSharp.kernel-memory) package (this package only supports `net6.0` or higher yet), which is based on [Microsoft kernel-memory](https://github.com/microsoft/kernel-memory) integration.

### Model preparation

There are two popular format of model file of LLM now, which are PyTorch format (.pth) and Huggingface format (.bin). LLamaSharp uses `GGUF` format file, which could be converted from these two formats. To get `GGUF` file, there are two options:

1. Search model name + 'gguf' in [Huggingface](https://huggingface.co), you will find lots of model files that have already been converted to GGUF format. Please take care of the publishing time of them because some old ones could only work with old version of LLamaSharp.

2. Convert PyTorch or Huggingface format to GGUF format yourself. Please follow the instructions of [this part of llama.cpp readme](https://github.com/ggerganov/llama.cpp?tab=readme-ov-file#prepare-and-quantize) to convert them with the python scripts.

Generally, we recommend downloading models with quantization rather than fp16, because it significantly reduce the required memory size while only slightly impact on its generation quality.


### Example of LLaMA chat session

Here is a simple example to chat with bot based on LLM in LLamaSharp. Please replace the model path with yours.

```cs
using LLama.Common;
using LLama;

string modelPath = @"<Your Model Path>"; // change it to your own model path.

var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024, // The longest length of chat as memory.
    GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
};
using var model = LLamaWeights.LoadFromFile(parameters);
using var context = model.CreateContext(parameters);
var executor = new InteractiveExecutor(context);

// Add chat histories as prompt to tell AI how to act.
var chatHistory = new ChatHistory();
chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
chatHistory.AddMessage(AuthorRole.User, "Hello, Bob.");
chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

ChatSession session = new(executor, chatHistory);

InferenceParams inferenceParams = new InferenceParams()
{
    MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
    AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
};

Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write("The chat session has started.\nUser: ");
Console.ForegroundColor = ConsoleColor.Green;
string userInput = Console.ReadLine() ?? "";

while (userInput != "exit")
{
    await foreach ( // Generate the response streamingly.
        var text
        in session.ChatAsync(
            new ChatHistory.Message(AuthorRole.User, userInput),
            inferenceParams))
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(text);
    }
    Console.ForegroundColor = ConsoleColor.Green;
    userInput = Console.ReadLine() ?? "";
}
```

For more examples, please refer to [LLamaSharp.Examples](./LLama.Examples).


## 💡FAQ

#### Why GPU is not used when I have installed CUDA

1. If you are using backend packages, please make sure you have installed the cuda backend package which matches the cuda version of your device. Please note that before LLamaSharp v0.10.0, only one backend package should be installed.
2. Add `NativeLibraryConfig.Instance.WithLogs(LLamaLogLevel.Info)` to the very beginning of your code. The log will show which native library file is loaded. If the CPU library is loaded, please try to compile the native library yourself and open an issue for that. If the CUDA libraty is loaded, please check if `GpuLayerCount > 0` when loading the model weight.

#### Why the inference is slow

Firstly, due to the large size of LLM models, it requires more time to generate outputs than other models, especially when you are using models larger than 30B.

To see if that's a LLamaSharp performance issue, please follow the two tips below.

1. If you are using CUDA, Metal or OpenCL, please set `GpuLayerCount` as large as possible.
2. If it's still slower than you expect it to be, please try to run the same model with same setting in [llama.cpp examples](https://github.com/ggerganov/llama.cpp/tree/master/examples). If llama.cpp outperforms LLamaSharp significantly, it's likely a LLamaSharp BUG and please report us for that.


#### Why the program crashes before any output is generated

Generally, there are two possible cases for this problem:

1. The native library (backend) you are using is not compatible with the LLamaSharp version. If you compiled the native library yourself, please make sure you have checkouted llama.cpp to the corresponding commit of LLamaSharp, which could be found at the bottom of README.
2. The model file you are using is not compatible with the backend. If you are using a GGUF file downloaded from huggingface, please check its publishing time.

#### Why my model is generating output infinitely

Please set anti-prompt or max-length when executing the inference.


## 🙌Contributing

Any contribution is welcomed! There's a TODO list in [LLamaSharp Dev Project](https://github.com/orgs/SciSharp/projects/5) and you could pick an interesting one to start. Please read the [contributing guide](./CONTRIBUTING.md) for more information. 

You can also do one of the followings to help us make LLamaSharp better:

- Submit a feature request.
- Star and share LLamaSharp to let others know it.
- Write a blog or demo about LLamaSharp.
- Help to develop Web API and UI integration.
- Just open an issue about the problem you met!

## Join the community

Join our chat on [Discord](https://discord.gg/7wNVU65ZDY) (please contact Rinne to join the dev channel if you want to be a contributor).

Join [QQ group](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=sN9VVMwbWjs5L0ATpizKKxOcZdEPMrp8&authKey=RLDw41bLTrEyEgZZi%2FzT4pYk%2BwmEFgFcrhs8ZbkiVY7a4JFckzJefaYNW6Lk4yPX&noverify=0&group_code=985366726)

## Star history

[![Star History Chart](https://api.star-history.com/svg?repos=SciSharp/LLamaSharp)](https://star-history.com/#SciSharp/LLamaSharp&Date)

## Contributor wall of fame

[![LLamaSharp Contributors](https://contrib.rocks/image?repo=SciSharp/LLamaSharp)](https://github.com/SciSharp/LLamaSharp/graphs/contributors)

## Map of LLamaSharp and llama.cpp versions
If you want to compile llama.cpp yourself you **must** use the exact commit ID listed for each version.

| LLamaSharp | Verified Model Resources | llama.cpp commit id |
| - | -- | - |
| v0.2.0 | This version is not recommended to use. | - |
| v0.2.1 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama), [Vicuna (filenames with "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | - |
| v0.2.2, v0.2.3 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama_ggmlv2), [Vicuna (filenames without "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | `63d2046` |
| v0.3.0, v0.4.0 | [LLamaSharpSamples v0.3.0](https://huggingface.co/AsakusaRinne/LLamaSharpSamples/tree/v0.3.0), [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/main) | `7e4ea5b` |
| v0.4.1-preview | [Open llama 3b](https://huggingface.co/SlyEcho/open_llama_3b_ggml), [Open Buddy](https://huggingface.co/OpenBuddy/openbuddy-llama-ggml)| `aacdbd4` |
|v0.4.2-preview | [Llama2 7B (GGML)](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGML)| `3323112` |
| v0.5.1 | [Llama2 7B (GGUF)](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF)| `6b73ef1` |
| v0.6.0 | | [`cb33f43`](https://github.com/ggerganov/llama.cpp/commit/cb33f43a2a9f5a5a5f8d290dd97c625d9ba97a2f) |
| v0.7.0, v0.8.0 | [Thespis-13B](https://huggingface.co/TheBloke/Thespis-13B-v0.5-GGUF/tree/main?not-for-all-audiences=true), [LLaMA2-7B](https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF) | [`207b519`](https://github.com/ggerganov/llama.cpp/commit/207b51900e15cc7f89763a3bb1c565fe11cbb45d) |
| v0.8.1 | | [`e937066`](https://github.com/ggerganov/llama.cpp/commit/e937066420b79a757bf80e9836eb12b88420a218) |
| v0.9.0, v0.9.1 | [Mixtral-8x7B](https://huggingface.co/TheBloke/Mixtral-8x7B-v0.1-GGUF) | [`9fb13f9`](https://github.com/ggerganov/llama.cpp/blob/9fb13f95840c722ad419f390dc8a9c86080a3700) |
| v0.10.0 | [Phi2](https://huggingface.co/TheBloke/phi-2-GGUF) | [`d71ac90`](https://github.com/ggerganov/llama.cpp/tree/d71ac90985854b0905e1abba778e407e17f9f887) |
| v0.11.1, v0.11.2 | [LLaVA-v1.5](https://hf-mirror.com/jartine/llava-v1.5-7B-GGUF/blob/main/llava-v1.5-7b-mmproj-Q4_0.gguf), [Phi2](https://huggingface.co/TheBloke/phi-2-GGUF)| [`3ab8b3a`](https://github.com/ggerganov/llama.cpp/tree/3ab8b3a92ede46df88bc5a2dfca3777de4a2b2b6) |

## License

This project is licensed under the terms of the MIT license.
