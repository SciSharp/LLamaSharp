using LLama.Abstractions;

namespace LLama.Web.Models
{
    public class ModelSession : IDisposable
    {
        private bool _isFirstInteraction = true;
        private ModelOptions _modelOptions;
        private PromptOptions _promptOptions;
        private ParameterOptions _inferenceOptions;
        private ITextStreamTransform _outputTransform;
        private ILLamaExecutor _executor;
        private CancellationTokenSource _cancellationTokenSource;

        public ModelSession(string connectionId, ILLamaExecutor executor, ModelOptions modelOptions, PromptOptions promptOptions, ParameterOptions parameterOptions)
        {
            ConnectionId = connectionId;
            _executor = executor;
            _modelOptions = modelOptions;
            _promptOptions = promptOptions;
            _inferenceOptions = parameterOptions;
            
            _inferenceOptions.AntiPrompts = _promptOptions.AntiPrompt?.Concat(_inferenceOptions.AntiPrompts ?? Enumerable.Empty<string>()).Distinct() ?? _inferenceOptions.AntiPrompts;
            if (_promptOptions.OutputFilter?.Count > 0)
                _outputTransform = new LLamaTransforms.KeywordTextOutputStreamTransform(_promptOptions.OutputFilter, redundancyLength: 5);
        }

        public string ConnectionId { get; }

        public IAsyncEnumerable<string> InferAsync(string message, CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            if (_isFirstInteraction)
            {
                _isFirstInteraction = false;
                message = _promptOptions.Prompt + message;
            }

            if (_outputTransform is not null)
                return _outputTransform.TransformAsync(_executor.InferAsync(message, _inferenceOptions, _cancellationTokenSource.Token));

            return _executor.InferAsync(message, _inferenceOptions, _cancellationTokenSource.Token);
        }


        public void CancelInfer()
        {
            _cancellationTokenSource?.Cancel();
        }

        public bool IsInferCanceled()
        {
            return _cancellationTokenSource.IsCancellationRequested;
        }

        public void Dispose()
        {
            _inferenceOptions = null;
            _outputTransform = null;
            _executor.Model?.Dispose();
            _executor = null;
        }
    }
}
