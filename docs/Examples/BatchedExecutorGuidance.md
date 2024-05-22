# BatchedExecutor Guidance - Classifier Free Guidance / Negative Prompting

This example demonstrates using `Classifier Free Guidance` (a.k.a. negative prompting) with a custom sampling pipeline. Negative prompting is a way of steering the model output away from certain topics. See the source code [here](https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/Examples/BatchedExecutorGuidance.cs).

Two conversations are created. The `guided` conversation starts with the prompt that should be completed as shown as the output, for example `"my favourite colour is"`. The `guidance` conversation contains the negative prompt at the start, for example `"I hate the colour red. My favourite colour is"`. Note that this is a _negative_ prompt, so therefore this guidance will make the model answer as if it _likes_ the colour red.

A custom sampler samples the `guidance` conversation and uses that output to influence the output of the `guided` conversation. Once a token is selected _both_ conversations are continued with this token.