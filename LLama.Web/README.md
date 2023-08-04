## LLama.Web - Basic ASP.NET Core examples of LLamaSharp in action
LLama.Web has no heavy dependencies and no extra frameworks except bootstrap and jquery to keep the examples clean and easy to copy over to your own project


## Websockets
Using signalr websockets simplifys the streaming of responses and model per connection management


## Setup
You can setup Models, Ports etc in the appsettings.json

**Models**
You can add multiple models to the options for quick selection in the UI, options are based on ModelParams so its fully configurable


Example:
```json
  "LLamaOptions": {
    "Models": [
      {
        "Name": "WizardLM-7B",
        "MaxInstances": 2,
        "ModelPath": "\\Models\\wizardLM-7B.ggmlv3.q4_0.bin",
        "ContextSize": 2048
      },
      {
        "Name": "WizardLM-13B",
        "MaxInstances": 2,
        "ModelPath": "\\Models\\wizardLM-13B.ggmlv3.q4_0.bin",
        "ContextSize": 1024,
		"GpuLayerCount": 16,
		"Threads": 15
      }
    ]
  }
```


## Inference Demo
The Inference Demo UI is a simple example of using LLamaSharp
![demo-ui](https://i.imgur.com/FG0YEzw.png)


Inference Parameters
![demo-ui2](https://i.imgur.com/fZEQTQ5.png)
