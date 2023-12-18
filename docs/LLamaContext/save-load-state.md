# Save/Load State

There're two ways to load state: loading from path and loading from bite array. Therefore, correspondingly, state data can be extracted as byte array or saved to a file.

```cs
LLamaModel model = new LLamaModel(new ModelParams("<modelPath>"));
// do some things...
model.SaveState("model.st");
var stateData = model.GetStateData();
model.Dispose();

LLamaModel model2 = new LLamaModel(new ModelParams("<modelPath>"));
model2.LoadState(stateData);
// do some things...

LLamaModel model3 = new LLamaModel(new ModelParams("<modelPath>"));
model3.LoadState("model.st");
// do some things...
```