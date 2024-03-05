using LLama.Common;
using LLama.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    public class SafeLlavaModelHandle
    {
        LLava.LLavaContext handle;
        ModelParams parameters;
        internal protected SafeLlavaModelHandle(LLava.LLavaContext handle, ModelParams parameters)
        {
            this.handle = handle;
            this.parameters = parameters;
        }

        /// <inheritdoc />
        protected bool ReleaseHandle()
        {
            NativeApi.clip_free(handle.ClipContext);
            handle.model.Dispose();
            handle.LLamaContext.Dispose();
            return true;
        }

        /// <summary>
        /// Load a model from the given file path into memory
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlavaModelHandle LoadModel(ModelParams parameters, string clipModelPath)
        {
            LLamaWeights? model = LLamaWeights.LoadFromFile(parameters);
            LLamaContext? context = model.CreateContext(parameters);
            IntPtr clipContext = NativeApi.clip_model_load(clipModelPath, 1);
            if (clipContext == IntPtr.Zero)
                throw new RuntimeError($"Failed to load LLava Clip model {clipModelPath}.");

            return new SafeLlavaModelHandle(new LLava.LLavaContext { model = model.NativeHandle, LLamaContext = context.NativeHandle, ClipContext = clipContext }, parameters);
        }

        /// <summary>
        /// Evaluate the given prompt and image data
        /// </summary>
        /// <param name="prompt">prompt string to eval</param>
        /// <param name="imageData">Image data to eval</param>
        /// <returns>The tokens count from eval</returns>
        /// <exception cref="Exception"></exception>
        public int EvalPrompts(string prompt, byte[] imageData)
        {
            LLava.LLavaImageEmbed embedImg = NativeApi.llava_image_embed_make_with_bytes(this.handle.ClipContext, (int)parameters.Threads, imageData, imageData.Length);

            //int maxTgtLen = 256; /*params->n_predict < 0 ? 256 : params->n_predict;*/
            bool addBos = LLamaShouldAddBosToken();

            string QuesstionAnsweringPrompt = "A chat between a curious human and an artificial intelligence assistant. The assistant gives helpful, brief, and polite answers to the human's questions.\nUSER:";
            string AssistantPrompt = "\nASSISTANT:";
            int n_past = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            if (!EvalString(QuesstionAnsweringPrompt, ref n_past, addBos))
            {
                throw new Exception("Eval string Error");
            }
            else
            {
                Console.WriteLine($"Time to eval system prompt:  {stopwatch.ElapsedMilliseconds} ms");
            }
            stopwatch.Restart();
            if (!NativeApi.llava_eval_image_embed(this.handle.LLamaContext, embedImg, (int)parameters.BatchSize, ref n_past))
            {
                throw new Exception("Eval image embed Error");
            }
            else
            {
                Console.WriteLine($"Time to eval image: {stopwatch.ElapsedMilliseconds} ms");
            }
            stopwatch.Restart();
            if (!EvalString(prompt + AssistantPrompt, ref n_past, false))
            {
                throw new Exception("Eval string Error");
            }
            else
            {
                Console.WriteLine($"Time to eval textual prompt: {stopwatch.ElapsedMilliseconds} ms");
            }
            Console.WriteLine();
            return n_past;
        }

        private bool LLamaShouldAddBosToken()
        {
            int add_bos = NativeApi.llama_add_bos_token(this.handle.model);
            return (add_bos != 0) ? true : (NativeApi.llama_vocab_type(this.handle.model) == LLamaVocabType.LLAMA_VOCAB_TYPE_SPM);
        }

        private bool EvalString(string str, ref int n_past, bool add_bos)
        {
            string str2 = str;
            LLamaToken[] embd_inp = LLamaTokenize(str2, add_bos, true);
            EvalTokens(embd_inp, (int)this.parameters.BatchSize, ref n_past);
            return true;
        }

        unsafe
        private LLamaToken[] LLamaTokenize(string text, bool add_bos = false, bool special = true)
        {
            int n_tokens = text.Length + (add_bos ? 1 : 0);
            LLamaToken[] result = new LLamaToken[n_tokens];
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            fixed (LLamaToken* resultPtr = result)
            fixed (byte* bytesPtr = bytes)
            {
                // upper limit for the number of tokens
                n_tokens = NativeApi.llama_tokenize(this.handle.model, bytesPtr, text.Length, resultPtr, result.Length, add_bos, special);
                Array.Resize(ref result, Math.Abs(n_tokens));
                return result;
            }
        }

        unsafe
        private bool EvalTokens(LLamaToken[] tokens, int n_batch, ref int n_past)
        {
            int N = tokens.Length;
            for (int i = 0; i < N; i += n_batch)
            {
                int n_eval = tokens.Length - i;
                if (n_eval > n_batch)
                {
                    n_eval = n_batch;
                }
                fixed (LLamaToken* tokenPtr = tokens)
                {
                    //llama_batch batch = Native.llama_batch_get_one(intPtr, n_eval, ref n_past, 0);
                    LLamaNativeBatch batch = NativeApi.llama_batch_get_one(&tokenPtr[i], n_eval, ref n_past, 0);

                    //batch._all_pos_1 = 1;
                    batch._all_pos_0 = n_past;
                    int decodeResult = NativeApi.llama_decode(this.handle.LLamaContext, batch);
                    if (0 != decodeResult)
                    {
                        Console.WriteLine("failed to eval token\n");
                        return false;
                    }
                    n_past += n_eval;
                }
            }
            return true;
        }

        public void Infer(InferenceParams inferenceParams, int n_past)
        {
            LLamaSamplingParams samplingParams = new LLamaSamplingParams();
            samplingParams.temp = inferenceParams.Temperature;
            LLamaSamplingContext samplingContext = LLamaSamplingInit(samplingParams);

            //Is there any correlation among LLamaSamplingParams, InferenceParams and GPTParams?
            int maxTgtLen = 256; /*params->n_predict < 0 ? 256 : params->n_predict;*/
            for (int i = 0; i < maxTgtLen; i++)
            {
                string tmp = Sample(samplingContext, ref n_past);
                if (tmp == "</s>") break;
                if (tmp.Contains("###")) break; // Yi-VL behavior
                if (tmp.Contains("<|im_end|>")) break; // Yi-34B llava-1.6 - for some reason those decode not as the correct token (tokenizer works)
                if (tmp.Contains("<|im_start|>")) break; // Yi-34B llava-1.6
                if (tmp.Contains("USER:")) break; // mistral llava-1.6
                Console.Write(tmp);
            }
            //llama_sampling_free(ctx_sampling);
            //GCHandle handle = GCHandle.Alloc(ctx_sampling);
            //IntPtr ptr = GCHandle.ToIntPtr(handle);
            Console.WriteLine();
            NativeApi.llama_kv_cache_clear(this.handle.LLamaContext);
        }

        private LLamaSamplingContext LLamaSamplingInit(LLamaSamplingParams paramters)
        {
            LLamaSamplingContext result = new LLamaSamplingContext();

            result.parameters = paramters;
            result.grammar = IntPtr.Zero;
            Array.Resize(ref result.prev, paramters.n_prev);
            return result;
        }

        private string Sample(LLamaSamplingContext samplingContext, ref int n_past)
        {
            LLamaToken id = LLamaSamplingSample(samplingContext);
            LLamaSamplingAccept(ref samplingContext, id);
            string ret = string.Empty;
            if (id == NativeApi.llama_token_eos(this.handle.model))
            {
                ret = "</s>";
            }
            else
            {
                ret = llama_token_to_piece(id);
            }
            EvalId(id, ref n_past);
            return ret;
        }

        private LLamaToken LLamaSamplingSample(LLamaSamplingContext samplingContext, int idx = 0)
        {
            // Call the implementation function with is_resampling set to false by default
            return llama_sampling_sample_impl(samplingContext, idx, false);
        }

        unsafe
        private LLamaToken llama_sampling_sample_impl(LLamaSamplingContext samplingContext, int idx, bool isResampling)
        {  // Add a parameter to indicate if we are resampling
            LLamaSamplingParams parameters = samplingContext.parameters;

            int vocabCount = this.handle.LLamaContext.VocabCount;

            //float temp = @params.temp;
            int penalty_last_n = parameters.penalty_last_n < 0 ? parameters.n_prev : parameters.penalty_last_n;
            float penalty_repeat = parameters.penalty_repeat;
            float penalty_freq = parameters.penalty_freq;
            float penalty_present = parameters.penalty_present;
            int mirostat = parameters.mirostat;
            float mirostat_tau = parameters.mirostat_tau;
            float mirostat_eta = parameters.mirostat_eta;
            bool penalize_nl = parameters.penalize_nl;

            LLamaToken[] prev = samplingContext.prev;
            LLamaTokenData[] cur = samplingContext.cur;

            LLamaToken id = 0;

            // Get a pointer to the logits
            float* logits = NativeApi.llama_get_logits_ith(this.handle.LLamaContext, idx);

            // Declare original_logits at the beginning of the function scope
            cur = new LLamaTokenData[vocabCount];

            for (int token_id = 0; token_id < vocabCount; token_id++)
            {
                cur[token_id] = new LLamaTokenData { id = token_id, logit = logits[token_id], p = 0.0f };
            }
            //// apply penalties
            IntPtr cur_data = Marshal.UnsafeAddrOfPinnedArrayElement(cur, 0);
            LLamaTokenDataArrayNative cur_p = new LLamaTokenDataArrayNative { data = cur_data, size = (ulong)cur.Length, sorted = false };
            //LLamaToken[] penaltyTokens = parameters.use_penalty_prompt_tokens ? parameters.penalty_prompt_tokens : prev;

            //fixed (LLamaToken* penaltyTokensPtr = penaltyTokens)
            //{
            //    int penalty_tokens_used_size = Math.Min(penaltyTokens.Length, penalty_last_n);
            //    if (0 != penalty_tokens_used_size)
            //    {
            //        //float nl_logit = original_logits[Native.llama_token_nl(Native.llama_get_model(ctx_main))];

            //        NativeApi.llama_sample_repetition_penalties(this.handle.LLamaContext, ref cur_p,
            //           penaltyTokensPtr + penaltyTokens.Length - penalty_tokens_used_size,
            //             (ulong)penalty_tokens_used_size, penalty_repeat, penalty_freq, penalty_present);
            //    }
            //}

            int min_keep = Math.Max(1, parameters.n_probs);
            sampler_queue(parameters, ref cur_p, min_keep);
            id = NativeApi.llama_sample_token(handle.LLamaContext, ref cur_p);
            return id;

        }

        private void sampler_queue(LLamaSamplingParams @params, ref LLamaTokenDataArrayNative cur_p, int minKeep)
        {
            int n_vocab = this.handle.LLamaContext.VocabCount;

            float temp = @params.temp;
            float dynatemp_range = @params.dynatemp_range;
            float dynatemp_exponent = @params.dynatemp_exponent;
            int top_k = @params.top_k <= 0 ? n_vocab : @params.top_k;
            float top_p = @params.top_p;
            float min_p = @params.min_p;
            float tfs_z = @params.tfs_z;
            float typical_p = @params.typical_p;
            string samplers_sequence = @params.samplers_sequence;

            foreach (char s in samplers_sequence)
            {
                switch (s)
                {
                    case 'k': NativeApi.llama_sample_top_k(this.handle.LLamaContext, ref cur_p, top_k, (ulong)minKeep); break;
                    case 'f': NativeApi.llama_sample_tail_free(this.handle.LLamaContext, ref cur_p, tfs_z, (ulong)minKeep); break;
                    case 'y': NativeApi.llama_sample_typical(this.handle.LLamaContext, ref cur_p, typical_p, (ulong)minKeep); break;
                    case 'p': NativeApi.llama_sample_top_p(this.handle.LLamaContext, ref cur_p, top_p, (ulong)minKeep); break;
                    case 'm': NativeApi.llama_sample_min_p(this.handle.LLamaContext, ref cur_p, min_p, (ulong)minKeep); break;
                    case 't':
                        if (dynatemp_range > 0)
                        {
                            float dynatemp_min = Math.Max(0.0f, temp - dynatemp_range);
                            float dynatemp_max = Math.Max(0.0f, temp + dynatemp_range);
                            NativeApi.llama_sample_entropy(this.handle.LLamaContext, ref cur_p, dynatemp_min, dynatemp_max, dynatemp_exponent);
                        }
                        else
                        {
                            NativeApi.llama_sample_temp(this.handle.LLamaContext, ref cur_p, temp);
                        }
                        break;
                    default: break;
                }
            }

        }

        private void LLamaSamplingAccept(ref LLamaSamplingContext samplingContext, LLamaToken id)
        {
            List<LLamaToken> prev = samplingContext.prev.ToList();
            prev.RemoveAt(0);
            prev.Add(id);
            samplingContext.prev = prev.ToArray();
        }

        private string llama_token_to_piece(LLamaToken token)
        {
            string str = string.Empty;
            Span<byte> result = new byte[16];

            uint size = this.handle.LLamaContext.TokenToSpan(token, result);
            result = result.Slice(0, (int)size);
            str = Encoding.UTF8.GetString(result.ToArray());

            //int n_tokens = NativeApi.llama_token_to_piece(this.handle.model, token, result);
            //try
            //{
            //    if (n_tokens < 0)
            //    {
            //        result = result.Slice(0, -n_tokens);
            //        //Array.Resize(ref bytes, -n_tokens);
            //        NativeApi.llama_token_to_piece(this.handle.model, token, result);
            //    }
            //    else
            //    {
            //        result = result.Slice(0, n_tokens);
            //        //Array.Resize(ref result, n_tokens);
            //    }
            //    //return new string(result);
            //    str = Encoding.UTF8.GetString(result.ToArray(), 0, Math.Abs(n_tokens));

            //}
            //catch
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Error in llama_token_to_piece");
            //    Console.WriteLine(result.ToArray());
            //    Console.WriteLine(n_tokens);

            //    Console.WriteLine();
            //}
            return str;
        }

        private bool EvalId(LLamaToken id, ref int n_past)
        {
            LLamaToken[] tokens = new LLamaToken[] { id };
            return EvalTokens(tokens, 1, ref n_past);
        }

        public void Dispose()
        {
            ReleaseHandle();
            GC.SuppressFinalize(this);
        }   





    }
}
