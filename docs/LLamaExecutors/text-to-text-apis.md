# Text-to-Text APIs of the executors

All the executors implements the interface `ILLamaExecutor`, which provides two APIs to execute text-to-text tasks.

```cs
public interface ILLamaExecutor
{
    public LLamaModel Model { get; }

    IEnumerable<string> Infer(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);

    IAsyncEnumerable<string> InferAsync(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);
}
```

Just pass the text to the executor with the inference parameters. For the inference parameters, please refer to [executor inference parameters doc](./parameters.md).

The output of both two APIs are **yield enumerable**. Therefore, when receiving the output, you can directly use `foreach` to take actions on each word you get by order, instead of waiting for the whole process completed.