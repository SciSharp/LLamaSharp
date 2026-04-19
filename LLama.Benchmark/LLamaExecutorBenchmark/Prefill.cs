#pragma warning disable CS8618

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;

namespace LLama.Benchmark.LLamaExecutorBenchmark
{
#if WINDOWS
    [BenchmarkDotNet.Diagnostics.Windows.Configs.NativeMemoryProfiler]
#endif
    [BenchmarkCategory("Executor", "LLama")]
    [SimpleJob(RunStrategy.Monitoring, runtimeMoniker: RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [MinIterationCount(1)]
    [MaxIterationCount(16)]
    [RPlotExporter]
    public class PrefillBenchmark
    {
        /// <summary>
        /// (prompt length, context length)
        /// </summary>
        public IEnumerable<(int, uint)> PromptAndContextLengths => new (int, uint)[] 
        {
            (512, 2048),
            (2024, 2048)
        };

        /// <summary>
        /// (model path, gpu layer count)
        /// </summary>
        public IEnumerable<(string, int)> ModelAndGpuLayerCounts => new (string, int)[]
        // TODO: specify the native library to load here to test cpu case better.
        {
            (Path.Combine(Constants.ModelDir, Constants.Generative7BModelPath), 0),
            (Path.Combine(Constants.ModelDir, Constants.Generative7BModelPath), 10),
            (Path.Combine(Constants.ModelDir, Constants.Generative7BModelPath), 20)
        };

        public IEnumerable<ExecutorType> ExecutorTypes => new ExecutorType[]
        {
            ExecutorType.Interactive,
            ExecutorType.Stateless
        };

        [ParamsSource(nameof(PromptAndContextLengths))]
        public (int, uint) PromptAndContextLength { get; set; }

        [ParamsSource(nameof(ModelAndGpuLayerCounts))]
        public (string, int) ModelAndGpuLayerCount { get; set; }

        [ParamsSource(nameof(ExecutorTypes))]
        public ExecutorType ExecutorType { get; set; }

        /// <summary>
        /// Params used to create a model.
        /// </summary>
        public ModelParams ModelParams { get; set; }

        /// <summary>
        /// Params used in inference.
        /// </summary>
        public InferenceParams InferenceParams { get; set; }

        /// <summary>
        /// Prompt used to run text generation.
        /// </summary>
        public string Prompt { get; set; }

        public ILLamaExecutor Executor { get; set; }

        private void InitializeParamsAndModel()
        {
            ModelParams = new ModelParams(ModelAndGpuLayerCount.Item1)
            {
                ContextSize = PromptAndContextLength.Item2,
                GpuLayerCount = ModelAndGpuLayerCount.Item2
            };
            Prompt = File.ReadAllText(Constants.TextCompletionPromptsFilePath).Substring(0, PromptAndContextLength.Item1);
            InferenceParams = new InferenceParams()
            {
                MaxTokens = 1 // Only prefill, no generation here.
            };

            LLamaWeights weights = LLamaWeights.LoadFromFile(ModelParams);
            LLamaContext context = weights.CreateContext(ModelParams);
            Executor = ExecutorType switch
            {
                ExecutorType.Interactive => new InteractiveExecutor(context),
                ExecutorType.Instruct => new InstructExecutor(context),
                ExecutorType.Stateless => new StatelessExecutor(weights, ModelParams),
                _ => throw new NotSupportedException()
            };
        }

        [GlobalSetup(Targets = [nameof(Basic)])]
        public void GlobalSetup()
        {
            var showLLamaCppLogs = true;
            NativeLibraryConfig
               .All
               .WithLogCallback((level, message) =>
               {
                   if (showLLamaCppLogs)
                       Console.WriteLine($"[llama {level}]: {message.TrimEnd('\n')}");
               }).WithCuda().SkipCheck().WithAutoFallback(false);

            // Calling this method forces loading to occur now.
            NativeApi.llama_empty_call();
            InitializeParamsAndModel();
        }

        [IterationCleanup(Targets = [nameof(Basic)])]
        public void GlobalCleanup()
        {
            if(ExecutorType != ExecutorType.Stateless) // stateless executor always dispose its `Context` property
            {
                Executor.Context.NativeHandle.MemoryClear();
            }
        }

        [Benchmark]
        public async Task<string> Basic()
        {
            StringBuilder sb = new();
            await foreach(var text in Executor.InferAsync(Prompt, InferenceParams))
            {
                sb.Append(text);
            }
            return sb.ToString();
        }
    }
}
