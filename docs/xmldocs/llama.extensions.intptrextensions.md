[`< Back`](./)

---

# IntPtrExtensions

Namespace: LLama.Extensions

```csharp
public static class IntPtrExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IntPtrExtensions](./llama.extensions.intptrextensions.md)<br>
Attributes [ExtensionAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **PtrToStringWithDefault(IntPtr, String)**

Converts a native UTF-8 string pointer to a managed string, returning a fallback value when no data is available.

```csharp
public static string PtrToStringWithDefault(IntPtr ptr, string defaultValue = "")
```

#### Parameters

`ptr` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to a null-terminated UTF-8 string.

`defaultValue` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Value to return when the pointer is [IntPtr.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.intptr.zero) or when the string is empty.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Managed string representation of the native data, or `defaultValue` when unavailable.

### **PtrToString(IntPtr)**

Converts a pointer to a null-terminated UTF-8 string into a managed string.

```csharp
public static string? PtrToString(IntPtr ptr)
```

#### Parameters

`ptr` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the first byte of a null-terminated UTF-8 string.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>
Managed string representation, or `null` when the pointer is zero or the string is empty.

---

[`< Back`](./)
