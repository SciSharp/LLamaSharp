# Save/Load Chat Session

Generally, the chat session could be switched, which requires the ability of loading and saving session.

When building a chat bot app, it's **NOT encouraged** to initialize many chat sessions and keep them in memory to wait for being switched, because the memory consumption of both CPU and GPU is expensive. It's recommended to save the current session before switching to a new session, and load the file when switching back to the session.

The API is also quite simple, the files will be saved into a directory you specified. If the path does not exist, a new directory will be created.

```cs
string savePath = "<save dir>";
session.SaveSession(savePath);

session.LoadSession(savePath);
```
