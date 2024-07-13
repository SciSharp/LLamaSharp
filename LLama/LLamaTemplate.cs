using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LLama.Native;

namespace LLama;

/// <summary>
/// Converts a sequence of messages into text according to a model template
/// </summary>
public sealed class LLamaTemplate
{
    #region private state
    /// <summary>
    /// The model this template is for. May be null if a custom template was supplied to the constructor.
    /// </summary>
    private readonly SafeLlamaModelHandle? _model;

    /// <summary>
    /// Custom template. May be null if a model was supplied to the constructor.
    /// </summary>
    private readonly byte[]? _customTemplate;

    /// <summary>
    /// Keep a cache of roles converted into bytes. Roles are very frequently re-used, so this saves converting them many times.
    /// </summary>
    private readonly Dictionary<string, ReadOnlyMemory<byte>> _roleCache = [];

    /// <summary>
    /// Array of messages. The <see cref="Count"/> property indicates how many messages there are
    /// </summary>
    private TextMessage[] _messages = new TextMessage[4];

    /// <summary>
    /// Backing field for <see cref="AddAssistant"/>
    /// </summary>
    private bool _addAssistant;

    /// <summary>
    /// Temporary array of messages in the format llama.cpp needs, used when applying the template
    /// </summary>
    private LLamaChatMessage[] _nativeChatMessages = new LLamaChatMessage[4];

    /// <summary>
    /// Indicates how many bytes are in <see cref="_result"/> array
    /// </summary>
    private int _resultLength;

    /// <summary>
    /// Result bytes of last call to <see cref="Apply"/>
    /// </summary>
    private byte[] _result = [];

    /// <summary>
    /// Indicates if this template has been modified and needs regenerating
    /// </summary>
    private bool _dirty = true;
    #endregion

    #region properties
    /// <summary>
    /// The encoding algorithm to use
    /// </summary>
    public static readonly Encoding Encoding = Encoding.UTF8;

    /// <summary>
    /// Number of messages added to this template
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Get the message at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is less than zero or greater than or equal to <see cref="Count"/></exception>
    public TextMessage this[int index]
    {
        get
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be >= 0");
            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be < Count");

            return _messages[index]!;
        }
    }

    /// <summary>
    /// Whether to end the prompt with the token(s) that indicate the start of an assistant message.
    /// </summary>
    public bool AddAssistant
    {
        get => _addAssistant;
        set
        {
            if (value != _addAssistant)
            {
                _dirty = true;
                _addAssistant = value;
            }
        }
    }
    #endregion

    #region construction
    /// <summary>
    /// Construct a new template, using the default model template
    /// </summary>
    /// <param name="model"></param>
    public LLamaTemplate(SafeLlamaModelHandle model)
    {
        _model = model;
    }

    /// <summary>
    /// Construct a new template, using the default model template
    /// </summary>
    /// <param name="weights"></param>
    public LLamaTemplate(LLamaWeights weights)
        : this(weights.NativeHandle)
    {
    }

    /// <summary>
    /// Construct a new template, using a custom template.
    /// </summary>
    /// <remarks>Only support a pre-defined list of templates. See more: https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template</remarks>
    /// <param name="customTemplate"></param>
    public LLamaTemplate(string customTemplate)
    {
        _customTemplate = Encoding.GetBytes(customTemplate + "\0");
    }
    #endregion

    #region modify
    /// <summary>
    /// Add a new message to the end of this template
    /// </summary>
    /// <param name="role"></param>
    /// <param name="content"></param>
    /// <returns>This template, for chaining calls.</returns>
    public LLamaTemplate Add(string role, string content)
    {
        return Add(new TextMessage(role, content, _roleCache));
    }

    /// <summary>
    /// Add a new message to the end of this template
    /// </summary>
    /// <param name="message"></param>
    /// <returns>This template, for chaining calls.</returns>
    public LLamaTemplate Add(TextMessage message)
    {
        // Expand messages array if necessary
        if (Count == _messages.Length)
            Array.Resize(ref _messages, _messages.Length * 2);

        // Add message
        _messages[Count] = message;
        Count++;

        // Mark as dirty to ensure template is recalculated
        _dirty = true;

        return this;
    }

    /// <summary>
    /// Remove a message at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns>This template, for chaining calls.</returns>
    public LLamaTemplate RemoveAt(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be greater than or equal to zero");
        if (index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than Count");

        _dirty = true;
        Count--;

        // Copy all items after index down by one
        if (index < Count)
            Array.Copy(_messages, index + 1, _messages, index, Count - index);

        _messages[Count] = default;

        return this;
    }
    
    /// <summary>
    /// Remove all messages from the template and resets internal state to accept/generate new messages
    /// </summary>
    public void Clear()
    {
        _messages = new TextMessage[4];
        Count = 0;

        _resultLength = 0;
        _result = [];
        _nativeChatMessages = new LLamaChatMessage[4];

        _dirty = true;
    }
    #endregion

    /// <summary>
    /// Apply the template to the messages and write it into the output buffer
    /// </summary>
    /// <returns>A span over the buffer that holds the applied template</returns>
    public ReadOnlySpan<byte> Apply()
    {
        // Recalculate template if necessary
        if (_dirty)
        {
            _dirty = false;

            using var group = new GroupDisposable();
            unsafe
            {
                // Convert all the messages
                var totalInputBytes = 0;
                if (_nativeChatMessages.Length < _messages.Length)
                    Array.Resize(ref _nativeChatMessages, _messages.Length);
                for (var i = 0; i < Count; i++)
                {
                    ref var m = ref _messages[i]!;
                    totalInputBytes += m.RoleBytes.Length + m.ContentBytes.Length;

                    // Pin byte arrays in place
                    var r = m.RoleBytes.Pin();
                    group.Add(r);
                    var c = m.ContentBytes.Pin();
                    group.Add(c);

                    _nativeChatMessages[i] = new LLamaChatMessage
                    {
                        role = (byte*)r.Pointer,
                        content = (byte*)c.Pointer
                    };
                }

                // Get an array that's twice as large as the amount of input, hopefully that's large enough!
                var output = ArrayPool<byte>.Shared.Rent(Math.Max(32, totalInputBytes * 2));
                try
                {
                    // Run templater and discover true length
                    var outputLength = ApplyInternal(_nativeChatMessages.AsSpan(0, Count), output);

                    // If length was too big for output buffer run it again
                    if (outputLength > output.Length)
                    {
                        // Array was too small, rent another one that's exactly the size needed
                        ArrayPool<byte>.Shared.Return(output, true);
                        output = ArrayPool<byte>.Shared.Rent(outputLength);

                        // Run again, but this time with an output that is definitely large enough
                        ApplyInternal(_nativeChatMessages.AsSpan(0, Count), output);
                    }

                    // Grow result buffer if necessary
                    if (_result.Length < outputLength)
                        Array.Resize(ref _result, Math.Max(_result.Length * 2, outputLength));

                    // Copy to result buffer
                    output.AsSpan(0, outputLength).CopyTo(_result);
                    _resultLength = outputLength;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(output, true);
                }
            }
        }

        // Now that the template has been applied and is in the result buffer, copy it to the dest
        return _result.AsSpan(0, _resultLength);

        unsafe int ApplyInternal(Span<LLamaChatMessage> messages, byte[] output)
        {
            fixed (byte* customTemplatePtr = _customTemplate)
            fixed (byte* outputPtr = output)
            fixed (LLamaChatMessage* messagesPtr = messages)
            {
                return NativeApi.llama_chat_apply_template(_model, customTemplatePtr, messagesPtr, (nuint)messages.Length, AddAssistant, outputPtr, output.Length);
            }
        }
    }

    /// <summary>
    /// A message that has been added to a template
    /// </summary>
    public readonly struct TextMessage
    {
        /// <summary>
        /// The "role" string for this message
        /// </summary>
        public string Role { get; }

        /// <summary>
        /// The text content of this message
        /// </summary>
        public string Content { get; }

        internal ReadOnlyMemory<byte> RoleBytes { get; }
        internal ReadOnlyMemory<byte> ContentBytes { get; }

        internal TextMessage(string role, string content, IDictionary<string, ReadOnlyMemory<byte>> roleCache)
        {
            Role = role;
            Content = content;

            // Get bytes for role from cache
            if (!roleCache.TryGetValue(role, out var roleBytes))
            {
                // Convert role. Add one to length so there is a null byte at the end.
                var rArr = new byte[Encoding.GetByteCount(role) + 1];
                var encodedRoleLength = Encoding.GetBytes(role.AsSpan(), rArr);
                Debug.Assert(rArr.Length == encodedRoleLength + 1);

                // Add to cache for future use.
                // To ensure the cache cannot grow infinitely add a hard limit to size.
                if (roleCache.Count < 128)
                {
                    roleCache.Add(role, rArr);
                    roleBytes = rArr;
                }
            }
            RoleBytes = roleBytes;

            // Convert content. Add one to length so there is a null byte at the end.
            var contentArray = new byte[Encoding.GetByteCount(content) + 1];
            var encodedContentLength = Encoding.GetBytes(content.AsSpan(), contentArray);
            Debug.Assert(contentArray.Length == encodedContentLength + 1);
            ContentBytes = contentArray;
        }

        /// <summary>
        /// Deconstruct this message into role and content
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        public void Deconstruct(out string role, out string content)
        {
            role = Role;
            content = Content;
        }
    }
}