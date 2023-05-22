using System;
using System.Collections.Generic;

namespace LLama
{
    using llama_token = Int32;
    public struct LLamaParams
    {
        public int seed; // RNG seed
        public int n_threads = Math.Max(Environment.ProcessorCount / 2, 1); // number of threads (-1 = autodetect)
        public int n_predict = -1; // new tokens to predict
        public int n_ctx = 512; // context size
        public int n_batch = 512; // batch size for prompt processing (must be >=32 to use BLAS)
        public int n_keep = 0; // number of tokens to keep from initial prompt
        public int n_gpu_layers = -1;   // number of layers to store in VRAM

        // sampling parameters
        public Dictionary<llama_token, float> logit_bias; // logit bias for specific tokens
        public int top_k = 40; // <= 0 to use vocab size
        public float top_p = 0.95f; // 1.0 = disabled
        public float tfs_z = 1.00f; // 1.0 = disabled
        public float typical_p = 1.00f; // 1.0 = disabled
        public float temp = 0.80f; // 1.0 = disabled
        public float repeat_penalty = 1.10f; // 1.0 = disabled
        public int repeat_last_n = 64; // last n tokens to penalize (0 = disable penalty, -1 = context size)
        public float frequency_penalty = 0.00f; // 0.0 = disabled
        public float presence_penalty = 0.00f; // 0.0 = disabled
        public int mirostat = 0; // 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
        public float mirostat_tau = 5.00f; // target entropy
        public float mirostat_eta = 0.10f; // learning rate

        public string model = "models/lamma-7B/ggml-model.bin"; // model path
        public string prompt = ""; // initial prompt (set to empty string for interactive mode)
        public string path_session = ""; // path to file for saving/loading model eval state
        public string input_prefix = ""; // string to prefix user inputs with
        public string input_suffix = ""; // string to suffix user inputs with
        public List<string> antiprompt; // string upon seeing which more user input is prompted

        public string lora_adapter = ""; // lora adapter path
        public string lora_base = ""; // base model path for the lora adapter

        public bool memory_f16 = true; // use f16 instead of f32 for memory kv
        public bool random_prompt = false; // randomize prompt if none provided
        public bool use_color = false; // use color to distinguish generations and inputs
        public bool interactive = false; // interactive mode
        public bool prompt_cache_all = false; // save user input and generations to prompt cache

        public bool embedding = false; // get only sentence embedding
        public bool interactive_first = false; // wait for user input immediately

        public bool instruct = false; // instruction mode (used for Alpaca models)
        public bool penalize_nl = true; // consider newlines as a repeatable token
        public bool perplexity = false; // compute perplexity over the prompt
        public bool use_mmap = true; // use mmap for faster loads
        public bool use_mlock = false; // use mlock to keep model in memory
        public bool mem_test = false; // compute maximum memory usage
        public bool verbose_prompt = false; // print prompt tokens before generation

        public LLamaParams(int seed = 0, int n_threads = -1, int n_predict = -1,
            int n_ctx = 512, int n_batch = 512, int n_keep = 0, int n_gpu_layers = -1,
            Dictionary<llama_token, float> logit_bias = null, int top_k = 40, float top_p = 0.95f,
            float tfs_z = 1.00f, float typical_p = 1.00f, float temp = 0.80f, float repeat_penalty = 1.10f,
            int repeat_last_n = 64, float frequency_penalty = 0.00f, float presence_penalty = 0.00f,
            int mirostat = 0, float mirostat_tau = 5.00f, float mirostat_eta = 0.10f,
            string model = "models/lamma-7B/ggml-model.bin", string prompt = "",
            string path_session = "", string input_prefix = "", string input_suffix = "",
            List<string> antiprompt = null, string lora_adapter = "", string lora_base = "",
            bool memory_f16 = true, bool random_prompt = false, bool use_color = false, bool interactive = false,
            bool prompt_cache_all = false, bool embedding = false, bool interactive_first = false, 
            bool instruct = false, bool penalize_nl = true,
            bool perplexity = false, bool use_mmap = true, bool use_mlock = false, bool mem_test = false,
            bool verbose_prompt = false)
        {
            this.seed = seed;
            if (n_threads != -1)
            {
                this.n_threads = n_threads;
            }
            this.n_predict = n_predict;
            this.n_ctx = n_ctx;
            this.n_batch = n_batch;
            this.n_keep = n_keep;
            this.n_gpu_layers = n_gpu_layers == -1 ? 20 : n_gpu_layers;

            if (logit_bias == null)
            {
                logit_bias = new Dictionary<llama_token, float>();
            }
            this.logit_bias = logit_bias;

            this.top_k = top_k;
            this.top_p = top_p;
            this.tfs_z = tfs_z;
            this.typical_p = typical_p;
            this.temp = temp;
            this.repeat_penalty = repeat_penalty;
            this.repeat_last_n = repeat_last_n;
            this.frequency_penalty = frequency_penalty;
            this.presence_penalty = presence_penalty;
            this.mirostat = mirostat;
            this.mirostat_tau = mirostat_tau;
            this.mirostat_eta = mirostat_eta;

            this.model = model;
            this.prompt = prompt;
            this.path_session = path_session;
            this.input_prefix = input_prefix;
            this.input_suffix = input_suffix;

            if (antiprompt == null)
            {
                antiprompt = new List<string>();
            }
            this.antiprompt = antiprompt;

            this.lora_adapter = lora_adapter;
            this.lora_base = lora_base;

            this.memory_f16 = memory_f16;
            this.random_prompt = random_prompt;
            this.use_color = use_color;
            this.interactive = interactive;
            this.prompt_cache_all = prompt_cache_all;

            this.embedding = embedding;
            this.interactive_first = interactive_first;

            this.instruct = instruct;
            this.penalize_nl = penalize_nl;
            this.perplexity = perplexity;
            this.use_mmap = use_mmap;
            this.use_mlock = use_mlock;
            this.mem_test = mem_test;
            this.verbose_prompt = verbose_prompt;
        }
    }
}
