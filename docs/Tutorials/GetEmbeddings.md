# Get embeddings

Getting the embeddings of a text in LLM is sometimes useful, for example, to train other MLP models.

To get the embeddings, please initialize a `LLamaEmbedder` and then call `GetEmbeddings`.

```cs
var embedder = new LLamaEmbedder(new ModelParams("<modelPath>"));
string text = "hello, LLM.";
float[] embeddings = embedder.GetEmbeddings(text);
```

The output is a float array. Note that the length of the array is related with the model you load. If you just want to get a smaller size embedding, please consider changing a model.