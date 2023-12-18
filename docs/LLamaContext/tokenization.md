# Tokenization/Detokenization

A pair of APIs to make conversion between text and tokens.

## Tokenization

The basic usage is to call `Tokenize` after initializing the model.

```cs
LLamaModel model = new LLamaModel(new ModelParams("<modelPath>"));
string text = "hello";
int[] tokens = model.Tokenize(text).ToArray();
```

Depending on different model (or vocab), the output will be various.

## Detokenization

Similar to tokenization, just pass an `IEnumerable<int>` to `Detokenize` method.

```cs
LLamaModel model = new LLamaModel(new ModelParams("<modelPath>"));
int[] tokens = new int[] {125, 2568, 13245};
string text = model.Detokenize(tokens);
```
