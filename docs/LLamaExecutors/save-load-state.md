# Save/Load State of Executor

Similar to `LLamaModel`, an executor also has its state, which can be saved and loaded. **Note that in most of cases, the state of executor and the state of the model should be loaded and saved at the same time.** 

To decouple the model and executor, we provide APIs to save/load state for model and executor respectively. However, during the inference, the processed information will leave footprint in `LLamaModel`'s native context. Therefore, if you just load a state from another executor but keep the model unmodified, some strange things may happen. So will loading model state only.

Is there a condition that requires to load one of them only? The answer is YES. For example, after resetting the model state, if you don't want the inference starting from the new position, leaving the executor unmodified is okay. But, anyway, this flexible usage may cause some unexpected behaviors, therefore please ensure you know what you're doing before using it in this way.

In the future version, we'll open the access for some variables inside the executor to support more flexible usages.

The APIs to load/save state of the executors is similar to that of `LLamaModel`. However, note that `StatelessExecutor` doesn't have such APIs because it's stateless itself. Besides, the output of `GetStateData` is an object of type `ExecutorBaseState`.

```cs
LLamaModel model = new LLamaModel(new ModelParams("<modelPath>"));
InteractiveExecutor executor = new InteractiveExecutor(model);
// do some things...
executor.SaveState("executor.st");
var stateData = model.GetStateData();

InteractiveExecutor executor2 = new InteractiveExecutor(model);
executor2.LoadState(stateData);
// do some things...

InteractiveExecutor executor3 = new InteractiveExecutor(model);
executor3.LoadState("executor.st");
// do some things...
```