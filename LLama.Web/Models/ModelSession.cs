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
        private InteractiveExecutor _interactiveExecutor;
        private CancellationTokenSource _cancellationTokenSource;

        public ModelSession(string connectionId, ModelOptions modelOptions, PromptOptions promptOptions, ParameterOptions parameterOptions)
        {
            ConnectionId = connectionId;
            _modelOptions = modelOptions;
            _promptOptions = promptOptions;
            _inferenceOptions = parameterOptions;
            
            _interactiveExecutor = new InteractiveExecutor(new LLamaModel(_modelOptions));
            _inferenceOptions.AntiPrompts = _promptOptions.AntiPrompt?.Concat(_inferenceOptions.AntiPrompts ?? Enumerable.Empty<string>()).Distinct() ?? _inferenceOptions.AntiPrompts;
            if (_promptOptions.OutputFilter?.Count > 0)
                _outputTransform = new LLamaTransforms.KeywordTextOutputStreamTransform(_promptOptions.OutputFilter, redundancyLength: 5);
        }

        public string ConnectionId { get; }
        public string ResponseId { get; set; }

        public IAsyncEnumerable<string> InferAsync(string message, CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            if (_isFirstInteraction)
            {
                _isFirstInteraction = false;
                message = _promptOptions.Prompt + message;
            }

            if (_outputTransform is not null)
                return _outputTransform.TransformAsync(_interactiveExecutor.InferAsync(message, _inferenceOptions, _cancellationTokenSource.Token));

            return _interactiveExecutor.InferAsync(message, _inferenceOptions, _cancellationTokenSource.Token);
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
            _interactiveExecutor.Model?.Dispose();
            _interactiveExecutor = null;
        }
    }
}
