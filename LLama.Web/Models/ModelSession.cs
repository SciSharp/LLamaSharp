using LLama.Abstractions;
using LLama.Sampling;
using LLama.Web.Common;

namespace LLama.Web.Models;

public class ModelSession
{
    private readonly string _sessionId;
    private readonly LLamaModel _model;
    private readonly LLamaContext _context;
    private readonly ILLamaExecutor _executor;
    private readonly ISessionConfig _sessionConfig;
    private readonly ITextStreamTransform _outputTransform;
    private readonly InferenceOptions _defaultInferenceConfig;

    private CancellationTokenSource _cancellationTokenSource;

    public ModelSession(LLamaModel model, LLamaContext context, string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceOptions = null)
    {
        _model = model;
        _context = context;
        _sessionId = sessionId;
        _sessionConfig = sessionConfig;
        _defaultInferenceConfig = inferenceOptions ?? new InferenceOptions
        {
            SamplingPipeline = new DefaultSamplingPipeline()
        };
        _outputTransform = CreateOutputFilter();
        _executor = CreateExecutor();
    }

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    public string SessionId => _sessionId;

    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string ModelName => _sessionConfig.Model;

    /// <summary>
    /// Gets the context.
    /// </summary>
    public LLamaContext Context => _context;

    /// <summary>
    /// Gets the session configuration.
    /// </summary>
    public ISessionConfig SessionConfig => _sessionConfig;

    /// <summary>
    /// Gets the inference parameters.
    /// </summary>
    public InferenceOptions InferenceParams => _defaultInferenceConfig;

    /// <summary>
    /// Initializes the prompt.
    /// </summary>
    /// <param name="inferenceConfig">The inference configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async Task InitializePrompt(InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        if (_sessionConfig.ExecutorType == LLamaExecutorType.Stateless)
            return;

        if (string.IsNullOrEmpty(_sessionConfig.Prompt))
            return;

        // Run Initial prompt
        var inferenceParams = ConfigureInferenceParams(inferenceConfig);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await foreach (var _ in _executor.InferAsync(_sessionConfig.Prompt, inferenceParams, _cancellationTokenSource.Token))
        {
            // We dont really need the response of the initial prompt, so exit on first token
            break;
        };
    }

    /// <summary>
    /// Runs inference on the model context
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inferenceConfig">The inference configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    internal IAsyncEnumerable<string> InferAsync(string message, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        var inferenceParams = ConfigureInferenceParams(inferenceConfig);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var inferenceStream = _executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token);
        if (_outputTransform is not null)
            return _outputTransform.TransformAsync(inferenceStream);

        return inferenceStream;
    }

    public void CancelInfer()
    {
        _cancellationTokenSource?.Cancel();
    }

    public bool IsInferCanceled()
    {
        return _cancellationTokenSource.IsCancellationRequested;
    }

    /// <summary>
    /// Configures the inference parameters.
    /// </summary>
    /// <param name="inferenceConfig">The inference configuration.</param>
    private IInferenceParams ConfigureInferenceParams(InferenceOptions inferenceConfig)
    {
        var inferenceParams = inferenceConfig ?? _defaultInferenceConfig;
        inferenceParams.AntiPrompts = _sessionConfig.GetAntiPrompts();
        return inferenceParams;
    }

    private ITextStreamTransform CreateOutputFilter()
    {
        var outputFilters = _sessionConfig.GetOutputFilters();
        if (outputFilters.Count > 0)
            return new LLamaTransforms.KeywordTextOutputStreamTransform(outputFilters);

        return null;
    }

    private ILLamaExecutor CreateExecutor()
    {
        return _sessionConfig.ExecutorType switch
        {
            LLamaExecutorType.Interactive => new InteractiveExecutor(_context),
            LLamaExecutorType.Instruct => new InstructExecutor(_context),
            LLamaExecutorType.Stateless => new StatelessExecutor(_model.LLamaWeights, _context.Params),
            _ => default
        };
    }
}
