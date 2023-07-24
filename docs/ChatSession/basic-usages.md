# Basic usages of ChatSession

`ChatSession` is a higher-level abstraction than the executors. In the context of a chat application like ChatGPT, a "chat session" refers to an interactive conversation or exchange of messages between the user and the chatbot. It represents a continuous flow of communication where the user enters input or asks questions, and the chatbot responds accordingly. A chat session typically starts when the user initiates a conversation with the chatbot and continues until the interaction comes to a natural end or is explicitly terminated by either the user or the system. During a chat session, the chatbot maintains the context of the conversation, remembers previous messages, and generates appropriate responses based on the user's inputs and the ongoing dialogue.

## Initialize a session

Currently, the only parameter that is accepted is an `ILLamaExecutor`, because this is the only parameter that we're sure to exist in all the future versions. Since it's the high-level abstraction, we're conservative to the API designs. In the future, there may be more kinds of constructors added.

```cs
InteractiveExecutor ex = new(new LLamaModel(new ModelParams(modelPath)));
ChatSession session = new ChatSession(ex);
```

## Chat with the bot

There'll be two kinds of input accepted by the `Chat` API, which are `ChatHistory` and `String`. The API with string is quite similar to that of the executors. Meanwhile, the API with `ChatHistory` is aimed to provide more flexible usages. For example, you have had a chat with the bot in session A before you open the session B. Now session B has no memory for what you said before. Therefore, you can feed the history of A to B.

```cs
string prompt = "What is C#?";

foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } })) // the inference params should be changed depending on your statement
{
    Console.Write(text);
}
```

## Get the history

Currently `History` is a property of `ChatSession`.

```cs
foreach(var rec in session.History.Messages)
{
    Console.WriteLine($"{rec.AuthorRole}: {rec.Content}");
}
```
