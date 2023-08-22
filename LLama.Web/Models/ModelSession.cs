using LLama.Abstractions;
using LLama.Common;
using LLama.Web.Common;

namespace LLama.Web.Models
{
    public class ModelSession
    {
        private readonly LLamaContext _context;
        private readonly ILLamaExecutor _executor;
        private readonly SessionConfig _sessionParams;
        private readonly PromptConfig _promptParams;
        private readonly ITextStreamTransform _outputTransform;

        private IInferenceParams _inferenceParams;
        private CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSession"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        public ModelSession(LLamaContext context, SessionConfig sessionConfig, IInferenceParams inferenceParams = null)
        {
            _context = context;
            _sessionParams = sessionConfig;
            _inferenceParams = inferenceParams;

            // Executor
            _executor = sessionConfig.ExecutorType switch
            {
                LLamaExecutorType.Interactive => new InteractiveExecutor(_context),
                LLamaExecutorType.Instruct => new InstructExecutor(_context),
                LLamaExecutorType.Stateless => new StatelessExecutor(_context),
                _ => default
            };

            // Initial Prompt
            _promptParams = new PromptConfig
            {
                Prompt = _sessionParams.Prompt,
                AntiPrompt = CommaSeperatedToList(_sessionParams.AntiPrompt),
                OutputFilter = CommaSeperatedToList(_sessionParams.OutputFilter),
            };

            //Output Filter
            if (_promptParams.OutputFilter?.Count > 0)
                _outputTransform = new LLamaTransforms.KeywordTextOutputStreamTransform(_promptParams.OutputFilter, redundancyLength: 8);
        }


        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        public string ModelName => _sessionParams.Model;


        /// <summary>
        /// Initializes the prompt.
        /// </summary>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task InitializePrompt(IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            ConfigureInferenceParams(inferenceParams);

            if (_executor is StatelessExecutor)
                return;

            // Run Initial prompt
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await foreach (var _ in _executor.InferAsync(_sessionParams.Prompt, inferenceParams, _cancellationTokenSource.Token))
            {
                // We dont really need the response of the initial prompt, so exit on first token
                break;
            };
        }


        /// <summary>
        /// Runs inference on the model context
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public IAsyncEnumerable<string> InferAsync(string message, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            ConfigureInferenceParams(inferenceParams);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (_outputTransform is not null)
                return _outputTransform.TransformAsync(_executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token));

            return _executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token);
        }


        /// <summary>
        /// Cancels the current inference.
        /// </summary>
        public void CancelInfer()
        {
            _cancellationTokenSource?.Cancel();
        }


        /// <summary>
        /// Determines whether inference is canceled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if inference is canceled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInferCanceled()
        {
            return _cancellationTokenSource?.IsCancellationRequested ?? false;
        }

 
        /// <summary>
        /// Configures the inference parameters.
        /// </summary>
        /// <param name="inferenceParams">The inference parameters.</param>
        private void ConfigureInferenceParams(IInferenceParams inferenceParams)
        {
            // If not null set as default
            if (inferenceParams is not null)
                _inferenceParams = inferenceParams;

            // If null set to new
            if (_inferenceParams is null)
                _inferenceParams = new InferenceParams();

            // Merge Antiprompts
            var antiPrompts = new List<string>();
            antiPrompts.AddRange(_promptParams.AntiPrompt ?? Enumerable.Empty<string>());
            antiPrompts.AddRange(_inferenceParams.AntiPrompts ?? Enumerable.Empty<string>());
            _inferenceParams.AntiPrompts = antiPrompts.Distinct();
        }


        private static List<string> CommaSeperatedToList(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim())
                 .ToList();
        }
    }
}
