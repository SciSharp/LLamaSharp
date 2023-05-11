# LLamaSharp - .NET Bindings for llama.cpp

![logo](Assets/LLamaSharpLogo.png)

The C#/.NET binding of llama.cpp. It provides APIs to inference the LLaMa Models and deploy it on native environment or Web. It works on 
both Windows and Linux and does NOT require compiling the library yourself.

## Installation

Just search `LLama` in nuget package manager and install it!

```
PM> Install-Package LLama
```

## Usages

Currently, `LLamaSharp` provides two kinds of model, `LLamaModelV1` and `LLamaModel`. Both of them works but `LLamaModel` is more recommended 
because it provides better alignment with the master branch of [llama.cpp](https://github.com/ggerganov/llama.cpp).

Besides, `ChatSession` makes it easier to wrap your own chat bot. The code below is a simple example. For all examples, please refer to 
[Examples](./LLama.Examples).

```cs

var model = new LLamaModel(new LLamaParams(model: "<Your path>", n_ctx: 512, repeat_penalty: 1.0f));
var session = new ChatSession<LLamaModel>(model).WithPromptFile("<Your prompt file path>")
                .WithAntiprompt(new string[] { "User:" );
Console.Write("\nUser:");
while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    var question = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.White;
    var outputs = session.Chat(question); // It's simple to use the chat API.
    foreach (var output in outputs)
    {
        Console.Write(output);
    }
}
```

## Demo

![demo-console](Assets/console_demo.gif)

## Roadmap

✅ LLaMa model inference.

✅ Embeddings generation.

✅ Chat session.

🔳 Quantization

🔳 ASP.NET core Integration

🔳 WPF UI Integration

## Assets

The model weights is too large to include in the project. However some resources could be found below:

- [eachadea/ggml-vicuna-13b-1.1](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main)
- [TheBloke/wizardLM-7B-GGML](https://huggingface.co/TheBloke/wizardLM-7B-GGML)
- Magnet: [magnet:?xt=urn:btih:b8287ebfa04f879b048d4d4404108cf3e8014352&dn=LLaMA](magnet:?xt=urn:btih:b8287ebfa04f879b048d4d4404108cf3e8014352&dn=LLaMA)

The weights included in the magnet is exactly the weights from [Facebook LLaMa](https://github.com/facebookresearch/llama).

The prompts could be found below:
- [llama.cpp prompts](https://github.com/ggerganov/llama.cpp/tree/master/prompts) 
- [ChatGPT_DAN](https://github.com/0xk1h0/ChatGPT_DAN)
- [awesome-chatgpt-prompts-zh](https://github.com/PlexPt/awesome-chatgpt-prompts-zh)

## License

This project is licensed under the terms of the MIT license.