## LLama.Web - Basic ASP.NET Core examples of LLamaSharp in action
LLama.Web has no heavy dependencies and no extra frameworks ove bootstrap and jquery to keep the examples clean and easy to copy over to your own project

## Websockets
Using signalr websockets simplifys the streaming of responses and model per connection management



## Setup
You can setup Models, Prompts and Inference parameters in the appsettings.json

**Models**
You can add multiple models to the options for quick selection in the UI, options are based on ModelParams so its fully configurable

**Parameters**
You can add multiple sets of inference parameters to the options for quick selection in the UI, options are based on InferenceParams so its fully configurable

**Prompts**
You can add multiple sets of prompts to the options for quick selection in the UI

Example:
```json
 {
        "Name": "Alpaca",
        "Path": "D:\\Repositories\\AI\\Prompts\\alpaca.txt",
        "Prompt": "Alternativly to can set a prompt text directly and omit the Path"
        "AntiPrompt": [
          "User:"
        ],
        "OutputFilter": [
          "Response:",
          "User:"
        ]
  }
```


## Interactive UI
The interactive UI is a simple example of using LLammaSharp
![demo-ui](https://i.imgur.com/nQsnWP1.png)

