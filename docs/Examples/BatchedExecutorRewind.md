# BatchedExecutor - Rewind

This example demonstrates using the `BatchedExecutor` to split one sequence into multiple sequences. See the source code [here](https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/Examples/BatchedExecutorRewind.cs).

A single conversation is prompted and then continued for 24 tokens, after that it is re-wound by 12 tokens and continued from there. Rewinding simply sets the conversation back to an earlier state and requires no extra computation.