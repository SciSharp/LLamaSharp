# Overview

![logo](./media/LLamaSharpLogo.png)

LLamaSharp is the C#/.NET binding of [llama.cpp](https://github.com/ggerganov/llama.cpp). It provides APIs to inference the LLaMa Models and deploy it on native environment or Web. It could help C# developers to deploy the LLM (Large Language Model) locally and integrate with C# apps.

## Main features

- Model inference
- Model quantization
- Generating embeddings
- Grammar parse
- Interactive/Instruct/Stateless executor mode
- Chat session APIs
- Save/load the state
- Integration with other applications like BotSharp and semantic-kernel

## Essential insights for novice learners

If you are new to LLM, here're some tips for you to help you to get start with `LLamaSharp`. If you are experienced in this field, we'd still recommend you to take a few minutes to read it because some things perform differently compared to cpp/python.

1. The main ability of LLamaSharp is to provide an efficient way to run inference of LLM (Large Language Model) locally (and fine-tune model in the future). The model weights, however, need to be downloaded from other resources such as [huggingface](https://huggingface.co).
2. Since LLamaSharp supports multiple platforms, The nuget package is split into `LLamaSharp` and `LLama.Backend`. After installing `LLamaSharp`, please install one of `LLama.Backend.Cpu`, `LLama.Backend.Cuda11` or `LLama.Backend.Cuda12`. If you use the source code, dynamic libraries can be found in `LLama/Runtimes`.
3. `LLaMa` originally refers to the weights released by Meta (Facebook Research). After that, many models are fine-tuned based on it, such as `Vicuna`, `GPT4All`, and `Pyglion`. Though all of these models are supported by LLamaSharp, some steps are necessary with different file formats. There're mainly three kinds of files, which are `.pth`, `.bin (ggml)`, `.bin (quantized)`. If you have the `.bin (quantized)` file, it could be used directly by LLamaSharp. If you have the `.bin (ggml)` file, you could use it directly but get higher inference speed after the quantization. If you have the `.pth` file, you need to follow [the instructions in llama.cpp](https://github.com/ggerganov/llama.cpp#prepare-data--run) to convert it to `.bin (ggml)` file at first.
4. LLamaSharp supports GPU acceleration, but it requires cuda installation. Please install cuda 11 or cuda 12 on your system before using LLamaSharp to enable GPU. If you have another cuda version, you could compile llama.cpp from source to get the dll. For building from source, please refer to [issue #5](https://github.com/SciSharp/LLamaSharp/issues/5).

## Welcome to join the development!

Community effort is always one of the most important things in open-source projects. Any contribution in any way is welcomed here. For example, the following things mean a lot for LLamaSharp:

1. Open an issue when you find something wrong.
2. Open an PR if you've fixed something. Even if just correcting a typo, it also makes great sense.
3. Help to optimize the documentation. 
4. Write an example or blog about how to integrate LLamaSharp with your APPs.
5. Ask for a missed feature and discuss with other developers.

If you'd like to get deeply involved in development, please touch us in discord channel or send email to `AsakusaRinne@gmail.com`. :)
