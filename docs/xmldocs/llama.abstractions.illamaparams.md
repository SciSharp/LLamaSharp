# ILLamaParams

Namespace: LLama.Abstractions

Convenience interface for implementing both type of parameters.

```csharp
public interface ILLamaParams : IModelParams, IContextParams
```

Implements [IModelParams](./llama.abstractions.imodelparams.md), [IContextParams](./llama.abstractions.icontextparams.md)

**Remarks:**

Mostly exists for backwards compatibility reasons, when these two were not split.
