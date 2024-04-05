// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "LLamaSharp intentionally exports the native llama.cpp API")]

[assembly: SuppressMessage("Style", "IDE0070:Use 'System.HashCode'", Justification = "Not compatible with netstandard2.0")]

[assembly: SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Not compatible with netstandard2.0")]
