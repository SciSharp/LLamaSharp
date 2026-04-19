# MTMD interactive mode

`MtmdInteractiveModeExecute` shows how to pair a multimodal projection with a text model so the chat loop can reason over images supplied at runtime. The sample lives in `LLama.Examples/Examples/MtmdInteractiveModeExecute.cs` and reuses the interactive executor provided by LLamaSharp.

## Workflow
- Resolve the model, multimodal projection, and sample image paths via `UserSettings`.
- Create `ModelParams` for the text model and capture the MTMD defaults with `MtmdContextParams.Default()`.
- Load the base model and context, then initialize `SafeMtmdWeights` with the multimodal projection file.
- Ask the helper for a media marker (`mtmdParameters.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>"`) and feed it into an `InteractiveExecutor`.

```cs
var mtmdParameters = MtmdContextParams.Default();

using var model = await LLamaWeights.LoadFromFileAsync(parameters);
using var context = model.CreateContext(parameters);

// Mtmd Init
using var clipModel = await SafeMtmdWeights.LoadFromFileAsync(
    multiModalProj,
    model,
    mtmdParameters);

var mediaMarker = mtmdParameters.MediaMarker
    ?? NativeApi.MtmdDefaultMarker()
    ?? "<media>";

var ex = new InteractiveExecutor(context, clipModel);
```

## Handling user input
- Prompts can include image paths wrapped in braces (for example `{c:/image.jpg}`); the loop searches for those markers with regular expressions.
- Every referenced file is loaded through `SafeMtmdWeights.LoadMedia`, producing `SafeMtmdEmbed` instances that are queued for the next tokenization call.
- When the user provides images, the executor clears its KV cache (`MemorySequenceRemove`) before replacing each brace-wrapped path in the prompt with the multimodal marker.
- The embeds collected for the current turn are copied into `ex.Embeds`, so the executor submits both the text prompt and the pending media to the helper before generation.

## Running the sample
1. Ensure the model and projection paths returned by `UserSettings` exist locally.
2. Start the example (for instance from the examples host application) and observe the initial description printed to the console.
3. Type text normally, or reference new images by including their path inside braces. Type `/exit` to end the conversation.

This walkthrough mirrors the logic in the sample so you can adapt it for your own multimodal workflows.
