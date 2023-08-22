## LLama.Web - Basic ASP.NET Core examples of LLamaSharp in action
LLama.Web has no heavy dependencies and no extra frameworks over bootstrap and jquery to keep the examples clean and easy to copy over to your own project

## Websockets
Using signalr websockets simplifys the streaming of responses and model per connection management

## Setup
You can setup Models and  parameters in the appsettings.json

**Models**
You can add multiple models to the options for quick selection in the UI, options are based on ModelParams so its fully configurable

Example:
```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"AllowedHosts": "*",
	"LLamaConfig": {
		"Models": [{
			"Name": "WizardLM-7B",
			"MaxInstances": 2,
			"ModelPath": "D:\\Repositories\\Models\\wizardLM-7B.ggmlv3.q4_0.bin",
			"ContextSize": 512,
			"BatchSize": 512,
			"Threads": -1,
			"GpuLayerCount": 20,
			"UseMemorymap": true,
			"UseMemoryLock": false,
			"MainGpu": 0,
			"LowVram": false,
			"Seed": 1686349486,
			"UseFp16Memory": true,
			"Perplexity": false,
			"ModelAlias": "unknown",
			"LoraAdapter": "",
			"LoraBase": "",
			"ConvertEosToNewLine": false,
			"EmbeddingMode": false,
			"TensorSplits": null,
			"GroupedQueryAttention": 1,
			"RmsNormEpsilon": 0.000005,
			"RopeFrequencyBase": 10000.0,
			"RopeFrequencyScale": 1.0,
			"MulMatQ": false
		}]
	}
}
```


## Interactive UI
The interactive UI is a simple example of using LLammaSharp
![demo-ui](https://i.imgur.com/nQsnWP1.png)

