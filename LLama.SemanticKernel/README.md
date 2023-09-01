# LLamaSharp.SemanticKernel

LLamaSharp.SemanticKernel are connections for [SemanticKernel](https://github.com/microsoft/semantic-kernel): an SDK for intergrating various LLM interfaces into a single implementation. With this, you can add local LLaMa queries as another connection point with your existing connections.

For reference on how to implement it, view the following examples: 

- [SemanticKernelChat](../LLama.Examples/NewVersion/SemanticKernelChat.cs)
- [SemanticKernelPrompt](../LLama.Examples/NewVersion/SemanticKernelPrompt.cs)

## ITextCompletion
```csharp
using var model = LLamaWeights.LoadFromFile(parameters);
// LLamaSharpTextCompletion can accept ILLamaExecutor. 
var ex = new StatelessExecutor(model, parameters);
var builder = new KernelBuilder();
builder.WithAIService<ITextCompletion>("local-llama", new LLamaSharpTextCompletion(ex), true);
```

## IChatCompletion
```csharp
using var model = LLamaWeights.LoadFromFile(parameters);
using var context = model.CreateContext(parameters);
// LLamaSharpChatCompletion requires InteractiveExecutor, as it's the best fit for the given command.
var ex = new InteractiveExecutor(context);
var chatGPT = new LLamaSharpChatCompletion(ex);
```
