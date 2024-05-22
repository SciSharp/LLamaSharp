# Frequently asked questions

Sometimes, your application with LLM and LLamaSharp may have unexpected behaviours. Here are some frequently asked questions, which may help you to deal with your problem.

## Why GPU is not used when I have installed CUDA

1. If you are using backend packages, please make sure you have installed the cuda backend package which matches the cuda version of your device. Please note that before LLamaSharp v0.10.0, only one backend package should be installed.
2. Add `NativeLibraryConfig.Instance.WithLogs(LLamaLogLevel.Info)` to the very beginning of your code. The log will show which native library file is loaded. If the CPU library is loaded, please try to compile the native library yourself and open an issue for that. If the CUDA library is loaded, please check if `GpuLayerCount > 0` when loading the model weight.

## Why the inference is slow

Firstly, due to the large size of LLM models, it requires more time to generate outputs than other models, especially when you are using models larger than 30B.

To see if that's a LLamaSharp performance issue, please follow the two tips below.

1. If you are using CUDA, Metal or OpenCL, please set `GpuLayerCount` as large as possible.
2. If it's still slower than you expect it to be, please try to run the same model with same setting in [llama.cpp examples](https://github.com/ggerganov/llama.cpp/tree/master/examples). If llama.cpp outperforms LLamaSharp significantly, it's likely a LLamaSharp BUG and please report us for that.


## Why the program crashes before any output is generated

Generally, there are two possible cases for this problem:

1. The native library (backend) you are using is not compatible with the LLamaSharp version. If you compiled the native library yourself, please make sure you have checkouted llama.cpp to the corresponding commit of LLamaSharp, which could be found at the bottom of README.
2. The model file you are using is not compatible with the backend. If you are using a GGUF file downloaded from huggingface, please check its publishing time.


## Why my model is generating output infinitely

Please set anti-prompt or max-length when executing the inference.

Anti-prompt can also be called as "Stop-keyword", which decides when to stop the response generation. Under interactive mode, the maximum tokens count is always not set, which makes the LLM generates responses infinitively. Therefore, setting anti-prompt correctly helps a lot to avoid the strange behaviours. For example, the prompt file `chat-with-bob.txt` has the following content:

```
Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.

User: Hello, Bob.
Bob: Hello. How may I help you today?
User: Please tell me the largest city in Europe.
Bob: Sure. The largest city in Europe is Moscow, the capital of Russia.
User:
```

Therefore, the anti-prompt should be set as "User:". If the last line of the prompt is removed, LLM will automatically generate a question (user) and a response (bob) for one time when running the chat session. Therefore, the antiprompt is suggested to be appended to the prompt when starting a chat session.

What if an extra line is appended? The string "User:" in the prompt will be followed with a char "\n". Thus when running the model, the automatic generation of a pair of question and response may appear because the anti-prompt is "User:" but the last token is "User:\n". As for whether it will appear, it's an undefined behaviour, which depends on the implementation inside the `LLamaExecutor`. Anyway, since it may leads to unexpected behaviors, it's recommended to trim your prompt or carefully keep consistent with your anti-prompt.
 
## How to run LLM with non-English languages

English is the most popular language in the world, and in the region of LLM. If you want to accept inputs and generate outputs of other languages, please follow the two tips below.

1. Ensure the model you selected is well-trained with data of your language. For example, [LLaMA](https://github.com/meta-llama/llama) (original) used few Chinese text during the pretrain, while [Chinese-LLaMA-Alpaca](https://github.com/ymcui/Chinese-LLaMA-Alpaca) finetuned LLaMA with a large amount of Chinese text data. Therefore, the quality of the output of Chinese-LLaMA-Alpaca is much better than that of LLaMA.

## Pay attention to the length of prompt

Sometimes we want to input a long prompt to execute a task. However, the context size may limit the inference of LLama model. Please ensure the inequality below holds.

$$ len(prompt) + len(response) < len(context) $$

In this inequality, `len(response)` refers to the expected tokens for LLM to generate.

## Choose models weight depending on you task

The differences between modes may lead to much different behaviours under the same task. For example, if you're building a chat bot with non-English, a fine-tuned model specially for the language you want to use will have huge effect on the performance.
