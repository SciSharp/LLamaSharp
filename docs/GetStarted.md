# Get Started

## Install packages

Firstly, search `LLamaSharp` in nuget package manager and install it.

```
PM> Install-Package LLamaSharp
```

Then, search and install one of the following backends:

```
LLamaSharp.Backend.Cpu
LLamaSharp.Backend.Cuda11
LLamaSharp.Backend.Cuda12
```

Here's the mapping of them and corresponding model samples provided by `LLamaSharp`. If you're not sure which model is available for a version, please try our sample model.

| LLamaSharp.Backend | LLamaSharp | Verified Model Resources | llama.cpp commit id |
| - | - | -- | - |
| - | v0.2.0 | This version is not recommended to use. | - |
| - | v0.2.1 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama), [Vicuna (filenames with "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | - |
| v0.2.2 | v0.2.2, v0.2.3 | [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/previous_llama_ggmlv2), [Vicuna (filenames without "old")](https://huggingface.co/eachadea/ggml-vicuna-13b-1.1/tree/main) | 63d2046 |
| v0.3.0 | v0.3.0 | [LLamaSharpSamples v0.3.0](https://huggingface.co/AsakusaRinne/LLamaSharpSamples/tree/v0.3.0), [WizardLM](https://huggingface.co/TheBloke/wizardLM-7B-GGML/tree/main) | 7e4ea5b |


## Download a model

One of the following models could be okay:

- LLaMA 🦙
- [Alpaca](https://github.com/ggerganov/llama.cpp#instruction-mode-with-alpaca)
- [GPT4All](https://github.com/ggerganov/llama.cpp#using-gpt4all)
- [Chinese LLaMA / Alpaca](https://github.com/ymcui/Chinese-LLaMA-Alpaca)
- [Vigogne (French)](https://github.com/bofenghuang/vigogne)
- [Vicuna](https://github.com/ggerganov/llama.cpp/discussions/643#discussioncomment-5533894)
- [Koala](https://bair.berkeley.edu/blog/2023/04/03/koala/)
- [OpenBuddy 🐶 (Multilingual)](https://github.com/OpenBuddy/OpenBuddy)
- [Pygmalion 7B / Metharme 7B](#using-pygmalion-7b--metharme-7b)
- [WizardLM](https://github.com/nlpxucan/WizardLM)

**Note that because `llama.cpp` is under fast development now and often introduce break changes, some model weights on huggingface which works under a version may be invalid with another version. If it's your first time to configure LLamaSharp, we'd like to suggest for using verified model weights in the table above.**

## Run the program

Please create a console program with dotnet runtime >= netstandard 2.0 (>= net6.0 is more recommended). Then, paste the following code to `program.cs`;

```cs
using LLama.Common;
using LLama;

string modelPath = "<Your model path>" // change it to your own model path
var prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:"; // use the "chat-with-bob" prompt here.

// Load model
var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024
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
while (true)
{
    foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
    {
        Console.Write(text);
    }

    Console.ForegroundColor = ConsoleColor.Green;
    prompt = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.White;
}
```

After starting it, you'll see the following outputs.

```
Please input your model path: D:\development\llama\weights\wizard-vicuna-13B.ggmlv3.q4_1.bin
llama.cpp: loading model from D:\development\llama\weights\wizard-vicuna-13B.ggmlv3.q4_1.bin
llama_model_load_internal: format     = ggjt v3 (latest)
llama_model_load_internal: n_vocab    = 32000
llama_model_load_internal: n_ctx      = 1024
llama_model_load_internal: n_embd     = 5120
llama_model_load_internal: n_mult     = 256
llama_model_load_internal: n_head     = 40
llama_model_load_internal: n_layer    = 40
llama_model_load_internal: n_rot      = 128
llama_model_load_internal: ftype      = 3 (mostly Q4_1)
llama_model_load_internal: n_ff       = 13824
llama_model_load_internal: n_parts    = 1
llama_model_load_internal: model size = 13B
llama_model_load_internal: ggml ctx size = 7759.48 MB
llama_model_load_internal: mem required  = 9807.48 MB (+ 1608.00 MB per state)
....................................................................................................
llama_init_from_file: kv self size  =  800.00 MB

Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.

User: Hello, Bob.
Bob: Hello. How may I help you today?
User: Please tell me the largest city in Europe.
Bob: Sure. The largest city in Europe is Moscow, the capital of Russia.
User:
```

Now, enjoy chatting with LLM!