# LLamaSharp

## LLama

[AntipromptProcessor](./llama.antipromptprocessor.md)

[ChatSession](./llama.chatsession.md)

[InstructExecutor](./llama.instructexecutor.md)

[InteractiveExecutor](./llama.interactiveexecutor.md)

[LLamaContext](./llama.llamacontext.md)

[LLamaEmbedder](./llama.llamaembedder.md)

[LLamaQuantizer](./llama.llamaquantizer.md)

[LLamaReranker](./llama.llamareranker.md)

[LLamaTemplate](./llama.llamatemplate.md)

[LLamaTransforms](./llama.llamatransforms.md)

[LLamaWeights](./llama.llamaweights.md)

[LLavaWeights](./llama.llavaweights.md)

[SessionState](./llama.sessionstate.md)

[StatefulExecutorBase](./llama.statefulexecutorbase.md)

[StatelessExecutor](./llama.statelessexecutor.md)

[StreamingTokenDecoder](./llama.streamingtokendecoder.md)

## LLama.Abstractions

[IContextParams](./llama.abstractions.icontextparams.md)

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)

[IInferenceParams](./llama.abstractions.iinferenceparams.md)

[ILLamaExecutor](./llama.abstractions.illamaexecutor.md)

[ILLamaParams](./llama.abstractions.illamaparams.md)

[IModelParams](./llama.abstractions.imodelparams.md)

[INativeLibrary](./llama.abstractions.inativelibrary.md)

[INativeLibrarySelectingPolicy](./llama.abstractions.inativelibraryselectingpolicy.md)

[ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)

[ITextTransform](./llama.abstractions.itexttransform.md)

[LLamaExecutorExtensions](./llama.abstractions.llamaexecutorextensions.md)

[MetadataOverride](./llama.abstractions.metadataoverride.md)

[MetadataOverrideConverter](./llama.abstractions.metadataoverrideconverter.md)

[TensorBufferOverride](./llama.abstractions.tensorbufferoverride.md)

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)

[TensorSplitsCollectionConverter](./llama.abstractions.tensorsplitscollectionconverter.md)

## LLama.Batched

[AlreadyPromptedConversationException](./llama.batched.alreadypromptedconversationexception.md)

[BatchedExecutor](./llama.batched.batchedexecutor.md)

[CannotModifyWhileRequiresInferenceException](./llama.batched.cannotmodifywhilerequiresinferenceexception.md)

[CannotSampleRequiresInferenceException](./llama.batched.cannotsamplerequiresinferenceexception.md)

[CannotSampleRequiresPromptException](./llama.batched.cannotsamplerequirespromptexception.md)

[CannotSaveWhileRequiresInferenceException](./llama.batched.cannotsavewhilerequiresinferenceexception.md)

[Conversation](./llama.batched.conversation.md)

[ConversationExtensions](./llama.batched.conversationextensions.md)

[ExperimentalBatchedExecutorException](./llama.batched.experimentalbatchedexecutorexception.md)

## LLama.Common

[AuthorRole](./llama.common.authorrole.md)

[ChatHistory](./llama.common.chathistory.md)

[FixedSizeQueue&lt;T&gt;](./llama.common.fixedsizequeue-1.md)

[InferenceParams](./llama.common.inferenceparams.md)

[MirostatType](./llama.common.mirostattype.md)

[ModelParams](./llama.common.modelparams.md)

## LLama.Exceptions

[GetLogitsInvalidIndexException](./llama.exceptions.getlogitsinvalidindexexception.md)

[LLamaDecodeError](./llama.exceptions.llamadecodeerror.md)

[LoadWeightsFailedException](./llama.exceptions.loadweightsfailedexception.md)

[MissingTemplateException](./llama.exceptions.missingtemplateexception.md)

[RuntimeError](./llama.exceptions.runtimeerror.md)

[TemplateNotFoundException](./llama.exceptions.templatenotfoundexception.md)

## LLama.Extensions

[IContextParamsExtensions](./llama.extensions.icontextparamsextensions.md)

[IModelParamsExtensions](./llama.extensions.imodelparamsextensions.md)

[SpanNormalizationExtensions](./llama.extensions.spannormalizationextensions.md)

## LLama.Native

[AvxLevel](./llama.native.avxlevel.md)

[DecodeResult](./llama.native.decoderesult.md)

[DefaultNativeLibrarySelectingPolicy](./llama.native.defaultnativelibraryselectingpolicy.md)

[EncodeResult](./llama.native.encoderesult.md)

[GGMLType](./llama.native.ggmltype.md)

[GPUSplitMode](./llama.native.gpusplitmode.md)

[ICustomSampler](./llama.native.icustomsampler.md)

[LLamaAttentionType](./llama.native.llamaattentiontype.md)

[LLamaBatch](./llama.native.llamabatch.md)

[LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)

[LLamaChatMessage](./llama.native.llamachatmessage.md)

[LLamaContextParams](./llama.native.llamacontextparams.md)

[LLamaFtype](./llama.native.llamaftype.md)

[LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)

[LLamaLogitBias](./llama.native.llamalogitbias.md)

[LLamaLogLevel](./llama.native.llamaloglevel.md)

[LLamaModelKvOverrideType](./llama.native.llamamodelkvoverridetype.md)

[LLamaModelMetadataOverride](./llama.native.llamamodelmetadataoverride.md)

[LLamaModelParams](./llama.native.llamamodelparams.md)

[LLamaModelQuantizeParams](./llama.native.llamamodelquantizeparams.md)

[LLamaModelTensorBufferOverride](./llama.native.llamamodeltensorbufferoverride.md)

[LLamaNativeBatch](./llama.native.llamanativebatch.md)

[LLamaPerfContextTimings](./llama.native.llamaperfcontexttimings.md)

[LLamaPoolingType](./llama.native.llamapoolingtype.md)

[LLamaPos](./llama.native.llamapos.md)

[LLamaRopeType](./llama.native.llamaropetype.md)

[LLamaSamplerChainParams](./llama.native.llamasamplerchainparams.md)

[LLamaSamplingTimings](./llama.native.llamasamplingtimings.md)

[LLamaSeqId](./llama.native.llamaseqid.md)

[LLamaToken](./llama.native.llamatoken.md)

[LLamaTokenAttr](./llama.native.llamatokenattr.md)

[LLamaTokenData](./llama.native.llamatokendata.md)

[LLamaTokenDataArray](./llama.native.llamatokendataarray.md)

[LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)

[LLamaVocabType](./llama.native.llamavocabtype.md)

[LLavaImageEmbed](./llama.native.llavaimageembed.md)

[LoraAdapter](./llama.native.loraadapter.md)

[NativeApi](./llama.native.nativeapi.md)

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)

[NativeLibraryFromPath](./llama.native.nativelibraryfrompath.md)

[NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)

[NativeLibraryName](./llama.native.nativelibraryname.md)

[NativeLibraryWithAvx](./llama.native.nativelibrarywithavx.md)

[NativeLibraryWithCuda](./llama.native.nativelibrarywithcuda.md)

[NativeLibraryWithMacOrFallback](./llama.native.nativelibrarywithmacorfallback.md)

[NativeLibraryWithVulkan](./llama.native.nativelibrarywithvulkan.md)

[NativeLogConfig](./llama.native.nativelogconfig.md)

[RopeScalingType](./llama.native.ropescalingtype.md)

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)

[SafeLLamaHandleBase](./llama.native.safellamahandlebase.md)

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)

[SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)

[SystemInfo](./llama.native.systeminfo.md)

[UnknownNativeLibrary](./llama.native.unknownnativelibrary.md)

## LLama.Sampling

[BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md)

[DefaultSamplingPipeline](./llama.sampling.defaultsamplingpipeline.md)

[Grammar](./llama.sampling.grammar.md)

[GreedySamplingPipeline](./llama.sampling.greedysamplingpipeline.md)

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)

[ISamplingPipelineExtensions](./llama.sampling.isamplingpipelineextensions.md)

## LLama.Transformers

[PromptTemplateTransformer](./llama.transformers.prompttemplatetransformer.md)
