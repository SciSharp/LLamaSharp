using LLama.Common;
using LLama.Examples.Extensions;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    public class CustomSampler
    {
        public static async Task Run()
        {
            var modelPath = UserSettings.GetModelPath();

            var parameters = new ModelParams(modelPath);
            using var model = await LLamaWeights.LoadFromFileAsync(parameters);

            var ex = new StatelessExecutor(model, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("In this example a custom sampling pipeline with a custom sampler stage is being used. This demonstrates how to customise the samplers used, and " +
                "how to create a completely custom sampler stage which modifies the logits or selects a token." +
                "" +
                "In this case the custom sampler stage removes the most likely token. This will probably produce bad results, it's just a demo!"
            );
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new CustomSamplingPipeline(),
                MaxTokens = 50
            };

            while (true)
            {
                Console.Write("\nQuestion: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Answer: ");
                prompt = $"Question: {prompt?.Trim()} Answer: ";
                await foreach (var text in ex.InferAsync(prompt, inferenceParams).Spinner())
                {
                    Console.Write(text);
                }
            }
        }
    }

    public class CustomSamplingPipeline
        : BaseSamplingPipeline
    {
        protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
        {
            var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            // Take only the 10 most likely tokens
            chain.AddTopK(10);

            // Remove the most likely token
            chain.AddCustom(new RemoveMostLikelyToken());

            // Select from the distribution
            chain.AddSoftmax();
            chain.AddDistributionSampler(42);

            return chain;
        }
    }

    public class RemoveMostLikelyToken
        : ICustomSampler
    {
        public string Name => "Remove Most Likely Token";

        public void Apply(ref LLamaTokenDataArrayNative tokenData)
        {
            // Doesn't make sense to run this stage if there is only one candidate left
            if (tokenData.Size <= 1)
                return;

            // Ensure token data is sorted, so most likely token is first.
            // Note that this is a descending sort, the  **largest** value is first.
            if (!tokenData.Sorted)
                tokenData.Data.Sort((a, b) => b.Logit.CompareTo(a.Logit));

            // Make the most likely token impossible to pick
            tokenData.Data[0].Logit = float.NegativeInfinity;
            
            // It's **critically** important to set this if the logits are no longer sorted after the custom 
            // sampler has run. If you're not sure, it's always safer to set it to false.
            //
            // In this case, because the first logit has just been set to negative infinity
            // the token data is definitely not sorted!
            tokenData.Sorted = false;
        }

        public void Accept(LLamaToken token)
        {
        }

        public void Reset()
        {
        }

        public ICustomSampler Clone()
        {
            return new RemoveMostLikelyToken();
        }

        public void Dispose()
        {
        }
    }
}
