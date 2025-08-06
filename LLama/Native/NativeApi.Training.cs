namespace LLama.Native;

public static partial class NativeApi
{
    ///// <summary>
    ///// function that returns whether or not a given tensor contains trainable parameters
    ///// </summary>
    ///// <param name="ggml_tensor"></param>
    ///// <param name="userdata"></param>
    ///// <returns></returns>
    //[return: MarshalAs(UnmanagedType.U1)]
    //private unsafe delegate bool llama_opt_param_filter(void* ggml_tensor, void* userdata);

    //private unsafe struct llama_opt_params
    //{
    //    uint n_ctx_train; // assumed context size post training, use context size specified in llama_context if 0

    //    llama_opt_param_filter param_filter; // callback for determining which tensors contain trainable parameters
    //    void* param_filter_ud;               // userdata for determining which tensors contain trainable parameters

    //    ggml_opt_get_optimizer_params get_opt_pars; // callback for calculating optimizer parameters

    //    void* get_opt_pars_ud;                     // userdata for calculating optimizer parameters
    //};

    //internal static extern void llama_opt_init(SafeLLamaContextHandle ctx, SafeLLamaContextHandle model, llama_opt_params @params);

    //internal static extern void llama_opt_epoch(SafeLLamaContextHandle ct,
    //                                            ggml_opt_dataset_t dataset,
    //                                            ggml_opt_result_t         result_train,
    //                                            ggml_opt_result_t result_eval,
    //                                            int64_t                   idata_split,
    //                                            ggml_opt_epoch_callback callback_train,
    //                                            ggml_opt_epoch_callback   callback_eval);
}