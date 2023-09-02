# Architecture

## Architecture of main functions

The figure below shows the core framework structure, which is separated to four levels.

- **LLamaContext**: The holder of a model which directly interact with native library and provide some basic APIs such as tokenization and embedding. Currently it includes three classes: `LLamaContext`, `LLamaEmbedder` and `LLamaQuantizer`.
- **LLamaExecutors**: Executors which define the way to run the LLama model. It provides text-to-text APIs to make it easy to use. Currently we provide three kinds of executors: `InteractiveExecutor`, `InstructuExecutor` and `StatelessExecutor`.
- **ChatSession**: A wrapping for `InteractiveExecutor` and `LLamaContext`, which supports interactive tasks and saving/re-loading sessions. It also provides a flexible way to customize the text process by `IHistoryTransform`, `ITextTransform` and `ITextStreamTransform`.
- **High-level Applications**: Some applications that provides higher-level integration. For example, [BotSharp](https://github.com/SciSharp/BotSharp) provides integration for vector search, Chatbot UI and Web APIs. [semantic-kernel](https://github.com/microsoft/semantic-kernel) provides various APIs for manipulations related with LLM. If you've made an integration, please tell us and add it to the doc!


![structure_image](media/structure.jpg)

## Recommended Use

Since `LLamaContext` interact with native library, it's not recommended to use the methods of it directly unless you know what you are doing. So does the `NativeApi`, which is not included in the architecture figure above.

`ChatSession` is recommended to be used when you want to build an application similar to ChatGPT, or the ChatBot, because it works best with `InteractiveExecutor`. Though other executors are also allowed to passed as a parameter to initialize a `ChatSession`, it's not encouraged if you are new to LLamaSharp and LLM.

High-level applications, such as BotSharp, are supposed to be used when you concentrate on the part not related with LLM. For example, if you want to deploy a chat bot to help you remember your schedules, using BotSharp may be a good choice.

Note that the APIs of the high-level applications may not be stable now. Please take it into account when using them.