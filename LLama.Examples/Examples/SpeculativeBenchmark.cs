using LLama.Common;
using LLama.Native;
using System.Diagnostics;

namespace LLama.Examples.Examples
{
    /// <summary>
    /// A utility class to benchmark and compare the Tokens-Per-Second (TPS) performance of 
    /// Standard Autoregressive generation versus Speculative Decoding (Dual-Model and MTP).
    /// </summary>
    public static class SpeculativeBenchmark
    {
        public record BenchmarkResult(
            string Name,
            int TotalTokens,
            double ElapsedSeconds,
            double TokensPerSecond,
            double AcceptanceRate
        );

        public static async Task RunAsync(
            string targetModelPath,
            string draftModelPath,
            string prompt = "The following is a list of numbers from 1 to 500: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,",
            int maxTokens = 128,
            int draftTokens = 16,
            bool useMtp = false)
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("         LLAMASHARP SPECULATIVE DECODING BENCHMARK        ");
            Console.WriteLine("==========================================================");
            Console.WriteLine($"Target Model : {targetModelPath}");
            Console.WriteLine($"Draft Model  : {(useMtp ? "[MTP Self-Drafting]" : draftModelPath)}");
            Console.WriteLine($"Draft Budget : {draftTokens} tokens per burst");
            Console.WriteLine($"Target Tokens: {maxTokens} tokens");
            Console.WriteLine("----------------------------------------------------------\n");

            var targetParams = new ModelParams(targetModelPath)
            {
                ContextSize = 4096, 
                BatchSize = 512,
                LoadMTP = useMtp,
                GpuLayerCount = -1,
                MainGpu = 0,
                UBatchSize = 512,
                SplitMode = GPUSplitMode.None,
                SeqMax = (uint)(draftTokens + 1),
                ContextType = LLamaContextType.Default,

                // NATIVE PERFORMANCE OPTIMIZERS
                // llama.cpp explicitly warns that KVUnified=true can cause bad performance when SeqMax > 1
                KVUnified = false,
                SwaFull = true
            };

            using var targetWeights = LLamaWeights.LoadFromFile(targetParams);

            // MTP AUTO-DETECT
            // For optimal performance, the draft budget should perfectly match the number 
            // of MTP projection heads baked into the model's metadata.
            if (useMtp)
            {
                int mtpHeads = 0;
                foreach (var kvp in targetWeights.Metadata)
                {
                    if (kvp.Key.EndsWith("nextn_predict_layers"))
                    {
                        if (int.TryParse(kvp.Value, out int heads))
                        {
                            mtpHeads = heads;
                            break;
                        }
                    }
                }

                if (mtpHeads > 0)
                {
                    Console.WriteLine($"\n[MTP Auto-Detect] Found {mtpHeads} MTP projection heads in the model metadata.");
                    if (draftTokens != mtpHeads)
                    {
                        Console.WriteLine($"[MTP Auto-Detect] Automatically adjusting draft budget from {draftTokens} to {mtpHeads} for optimal performance.");
                        draftTokens = mtpHeads;

                        // Update the SeqMax on our parameters BEFORE the Context is created
                        targetParams.SeqMax = (uint)(draftTokens + 1);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n[MTP Auto-Detect] Warning: Could not find 'nextn_predict_layers' in metadata. Proceeding with requested budget.");
                    Console.ResetColor();
                }
            }

            LLamaWeights? draftWeights = null;

            // CRITICAL CONTEXT SETUP

            // Check if the user provided a distinct draft file (prevents accidental double-loading)
            bool isSeparateDraft = !string.Equals(targetModelPath, draftModelPath, StringComparison.OrdinalIgnoreCase);

            // Always use draftModelPath here so Gemma 4 loads the correct assistant GGUF
            var draftParams = new ModelParams(draftModelPath)
            {
                ContextSize = 4096,
                BatchSize = 512,
                LoadMTP = useMtp,
                GpuLayerCount = -1,
                MainGpu = 0,
                UBatchSize = 512,
                SplitMode = GPUSplitMode.None,
                SeqMax = (uint)(draftTokens + 1),
                // In MTP mode, the secondary context MUST be explicitly flagged as LLamaContextType.Mtp
                ContextType = useMtp ? LLamaContextType.Mtp : LLamaContextType.Default,

                // NATIVE PERFORMANCE OPTIMIZERS
                KVUnified = false,
                SwaFull = true
            };
            // Important: In MTP mode, the executor will internally re-use the target weights for the draft context

            // Only load separate draft weights if the paths are actually different.
            // This supports Dual-Model AND split-MTP (Gemma 4), while keeping bundled-MTP (Qwen) safe.
            if (isSeparateDraft)
            {
                draftWeights = LLamaWeights.LoadFromFile(draftParams);
            }

            try
            {
                Console.WriteLine("[1/3] Warming up model & JIT compiler...");
                var warmupExecutor = new StatelessExecutor(targetWeights, targetParams);
                var warmupParams = new InferenceParams { MaxTokens = 10 };
                await foreach (var _ in warmupExecutor.InferAsync("Warmup test.", warmupParams)) { }
                Console.WriteLine("      Warmup complete.\n");

                Console.WriteLine($"[2/3] Running Speculative Decoding ({(useMtp ? "MTP" : "Draft-Simple")})...");
                // Important: In MTP mode, the executor will internally re-use the target weights for the 
                // draft context (draftWeights = null), but draftParams MUST be passed to trigger MTP context creation.
                var specExecutor = new StatelessExecutor(
                    weights: targetWeights,
                    @params: targetParams,
                    draftWeights: draftWeights,
                    draftParams: draftParams,
                    draftTokens: draftTokens,
                    useMtp: useMtp
                );

                var speculativeResult = await MeasureInferenceAsync(useMtp ? "MTP Speculative" : "Draft-Simple Speculative", specExecutor, prompt, maxTokens);

                Console.WriteLine("[3/3] Running Baseline: Standard Autoregressive Generation...");
                var standardExecutor = new StatelessExecutor(
                    weights: targetWeights,
                    @params: targetParams,
                    draftWeights: null,
                    draftParams: null,
                    draftTokens: 0,
                    useMtp: false
                );

                var baselineResult = await MeasureInferenceAsync("Standard Autoregressive", standardExecutor, prompt, maxTokens);

                PrintResultsTable(baselineResult, speculativeResult);
            }
            finally
            {
                draftWeights?.Dispose();
            }
        }

        public static async Task RunInteractiveAsync()
        {
            Console.WriteLine("=== Speculative Decoding Benchmark ===");

            // 1. Get Target Model
            Console.WriteLine("Select Target Model:");
            var targetModelPath = UserSettings.GetModelPath();

            // 2. Ask for Parameters dynamically
            var useMtp = Spectre.Console.AnsiConsole.Confirm("Does this model support MTP (e.g. Qwen3.5 / DeepSeek-R1 / Gemma-4)?");

            // It will default to 4 if MTP is true (good for Gemma), or 16 if MTP is false
            var draftTokens = Spectre.Console.AnsiConsole.Ask<int>("Enter draft tokens budget per burst:", useMtp ? 4 : 16);
            var maxTokens = Spectre.Console.AnsiConsole.Ask<int>("Enter max tokens to generate for the benchmark:", 128);

            // 3. Handle Draft Model path (Remove the !useMtp restriction)
            string draftModelPath = targetModelPath; // Default to self-speculation

            if (Spectre.Console.AnsiConsole.Confirm("Do you want to use a separate draft model? (Yes for Gemma 4 / Dual-Model, No for Qwen/DeepSeek)"))
            {
                Console.WriteLine("Select Draft Model:");
                draftModelPath = UserSettings.GetModelPath();
            }

            // 4. Run the benchmark with the explicitly chosen parameters
            await RunAsync(
                targetModelPath: targetModelPath,
                draftModelPath: draftModelPath,
                maxTokens: maxTokens,
                draftTokens: draftTokens,
                useMtp: useMtp
            );
        }

        private static async Task<BenchmarkResult> MeasureInferenceAsync(string testName, StatelessExecutor executor, string prompt, int maxTokens)
        {
            var inferenceParams = new InferenceParams { MaxTokens = maxTokens };
            int tokenCount = 0;
            var stopwatch = Stopwatch.StartNew();

            await foreach (var token in executor.InferAsync(prompt, inferenceParams))
            {
                tokenCount++;
            }

            stopwatch.Stop();
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            double tps = tokenCount / elapsedSeconds;

            double acceptanceRate = executor.AcceptanceRate;

            return new BenchmarkResult(testName, tokenCount, elapsedSeconds, tps, acceptanceRate);
        }

        private static void PrintResultsTable(BenchmarkResult baseline, BenchmarkResult speculative)
        {
            double speedupRatio = speculative.TokensPerSecond / baseline.TokensPerSecond;
            double percentageGain = (speedupRatio - 1.0) * 100.0;

            Console.WriteLine("\n==========================================================");
            Console.WriteLine("                    BENCHMARK RESULTS                     ");
            Console.WriteLine("==========================================================");
            Console.WriteLine($"{"Strategy",-28} | {"Tokens",-8} | {"Time (s)",-10} | {"Tokens/sec",-10} | {"Accept %",-8}");
            Console.WriteLine(new string('-', 75));
            Console.WriteLine($"{baseline.Name,-28} | {baseline.TotalTokens,-8} | {baseline.ElapsedSeconds,-10:F2} | {baseline.TokensPerSecond,-10:F2} | {"N/A",-8}");
            Console.WriteLine($"{speculative.Name,-28} | {speculative.TotalTokens,-8} | {speculative.ElapsedSeconds,-10:F2} | {speculative.TokensPerSecond,-10:F2} | {speculative.AcceptanceRate * 100,5:F1}%");
            Console.WriteLine(new string('-', 75));

            if (speedupRatio >= 1.0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Performance Result: {speedupRatio:F2}x Speedup (+{percentageGain:F1}%)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Performance Result: {speedupRatio:F2}x Slowdown ({percentageGain:F1}%)");
            }
            Console.ResetColor();
            Console.WriteLine("==========================================================\n");
        }
    }
}