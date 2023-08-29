# Tricks for FAQ

Sometimes, your application with LLM and LLamaSharp may have strange behaviours. Before opening an issue to report the BUG, the following tricks may worth a try.


## Carefully set the anti-prompts

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

## Pay attention to the length of prompt

Sometimes we want to input a long prompt to execute a task. However, the context size may limit the inference of LLama model. Please ensure the inequality below holds.

$$ len(prompt) + len(response) < len(context) $$

In this inequality, `len(response)` refers to the expected tokens for LLM to generate.

## Try different executors with a prompt

Some prompt works well under interactive mode, such as `chat-with-bob`, some others may work well with instruct mode, such as `alpaca`. Besides, if your input is quite simple and one-time job, such as "Q: what is the satellite of the earth? A: ", stateless mode will be a good choice.

If your chat bot has bad performance, trying different executor will possibly make it work well.

## Choose models weight depending on you task

The differences between modes may lead to much different behaviours under the same task. For example, if you're building a chat bot with non-English, a fine-tuned model specially for the language you want to use will have huge effect on the performance.

## Set the layer count you want to offload to GPU

Currently, the `GpuLayerCount` parameter, which decides the number of layer loaded into GPU, is set to 20 by default. However, if you have some efficient GPUs, setting it as a larger number will attain faster inference.