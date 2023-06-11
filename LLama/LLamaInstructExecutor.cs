using LLama.Abstractions.Params;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLama
{
    using llama_token = Int32;
    public class LLamaInstructExecutor : LLamaExecutorBase
    {
        bool _is_prompt_run = true;
        llama_token[] _inp_pfx;
        llama_token[] _inp_sfx;
        public LLamaInstructExecutor(LLamaModel model, string inputPrefix = "\n\n### Instruction:\n\n",
            string inputSuffix = "\n\n### Response:\n\n") : base(model)
        {
            _inp_pfx = _model.Tokenize(inputPrefix, true).ToArray();
            _inp_sfx = _model.Tokenize(inputSuffix, false).ToArray();
        }

        public override void SaveState(string filename)
        {
            InstructExecutorState state = new()
            {
                ConsumedSessionCount = _n_session_consumed,
                EmbedInps = _embed_inps,
                IsPromptRun = _is_prompt_run,
                ConsumedTokensCount = _consumedTokensCount,
                Embeds = _embeds,
                LastTokens = _last_n_tokens.ToArray(),
                InputPrefixTokens = _inp_pfx,
                InputSuffixTokens = _inp_sfx,
                MatchingSessionTokensCount = _n_matching_session_tokens,
                PastTokensCount = _pastTokensCount,
                SessionFilePath = _pathSession,
                SessionTokens = _session_tokens,
                LastTokensCapacity = _last_n_tokens.Capacity
            };
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                JsonSerializer.Serialize<InstructExecutorState>(fs, state);
            }
        }
        public override void LoadState(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = JsonSerializer.Deserialize<InstructExecutorState>(fs);
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps;
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds;
                _last_n_tokens = new FixedSizeQuene<llama_token>(state.LastTokensCapacity, state.LastTokens);
                _inp_pfx = state.InputPrefixTokens;
                _inp_sfx = state.InputSuffixTokens;
                _n_matching_session_tokens = state.MatchingSessionTokensCount;
                _pastTokensCount = state.PastTokensCount;
                _pathSession = state.SessionFilePath;
                _session_tokens = state.SessionTokens;
            }
        }

        protected override bool GetLoopCondition(InferStateArgs args)
        {
            return args.RemainedTokens != 0 || _is_prompt_run;
        }
        protected override void PreprocessInputs(string text, InferStateArgs args)
        {
            if (_is_prompt_run)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
                text = " " + text;
                _embed_inps = _model.Tokenize(text, true).ToList();
            }
            else
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                _consumedTokensCount = _embed_inps.Count;
                _embed_inps.AddRange(_inp_pfx);

                var line_inp = _model.Tokenize(text, false);
                _embed_inps.AddRange(line_inp);

                _embed_inps.AddRange(_inp_sfx);

                args.RemainedTokens -= line_inp.Count();
            }
        }
        protected override bool PostProcess(SessionParams sessionParams, InferStateArgs args, out IEnumerable<string>? extraOutputs)
        {
            extraOutputs = null;
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                {
                    string last_output = "";
                    foreach (var id in _last_n_tokens)
                    {
                        last_output += Utils.PtrToString(NativeApi.llama_token_to_str(_model.NativeHandle, id), _model.Encoding);
                    }

                    foreach (var antiprompt in args.Antiprompts)
                    {
                        if (last_output.EndsWith(antiprompt))
                        {
                            args.WaitForInput = true;
                            return true;
                        }
                    }
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    extraOutputs = new string[] { "\n> " };
                    return true;
                }
            }

            if (_embeds.Count > 0 && _embeds.Last() == NativeApi.llama_token_eos())
            {
                args.WaitForInput = true;
            }

            if (args.RemainedTokens <= 0 && sessionParams.ResponseTokensCount != -1)
            {
                args.RemainedTokens = sessionParams.ResponseTokensCount;
                args.WaitForInput = true;
            }
            return false;
        }
        protected override void InferInternal(SessionParams sessionParams, InferStateArgs args)
        {
            if (_embeds.Count > 0)
            {
                _is_prompt_run = false;
                if (_pastTokensCount + _embeds.Count > _model.ContextSize)
                {
                    HandleRunOutOfContext(sessionParams.TokensToKeep);
                }

                TryReuseMathingPrefix();
                _pastTokensCount = _model.Eval(_embeds.ToArray(), _pastTokensCount);

                if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                {
                    _session_tokens.AddRange(_embeds);
                    _n_session_consumed = _session_tokens.Count;
                }
            }

            _embeds.Clear();

            if (_embed_inps.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var temp = sessionParams.Temperature;
                var top_k = sessionParams.TopK <= 0 ? NativeApi.llama_n_vocab(_model.NativeHandle) : sessionParams.TopK;
                var top_p = sessionParams.TopK;
                var tfs_z = sessionParams.TfsZ;
                var typical_p = sessionParams.TypicalP;
                var repeat_last_n = sessionParams.RepeatLastTokensCount < 0 ? _model.ContextSize : sessionParams.RepeatLastTokensCount;
                var repeat_penalty = sessionParams.RepeatPenalty;
                var alpha_presence = sessionParams.PresencePenalty;
                var alpha_frequency = sessionParams.FrequencyPenalty;
                var mirostat = sessionParams.Mirostat;
                var mirostat_tau = sessionParams.MirostatTau;
                var mirostat_eta = sessionParams.MirostatEta;
                var penalize_nl = sessionParams.PenalizeNL;

                // optionally save the session on first sample (for faster prompt loading next time)
                if (!string.IsNullOrEmpty(_pathSession) && args.NeedToSaveSession)
                {
                    args.NeedToSaveSession = false;
                    SaveSessionFile(_pathSession);
                }

                var tokenDataArray = _model.ApplyPenalty(_last_n_tokens, sessionParams.LogitBias, repeat_last_n,
                    repeat_penalty, alpha_frequency, alpha_presence, penalize_nl);

                var id = _model.Sample(tokenDataArray, temp, mirostat, mirostat_tau, mirostat_eta, top_k, top_p,
                    tfs_z, typical_p);

                _last_n_tokens.Enqueue(id);

                _embeds.Add(id);

                args.RemainedTokens--;
                args.ReturnValue = true;
            }
            else
            {
                while (_embed_inps.Count > _consumedTokensCount)
                {
                    _embeds.Add(_embed_inps[_consumedTokensCount]);
                    _last_n_tokens.Enqueue(_embed_inps[_consumedTokensCount]);
                    _consumedTokensCount++;
                    if (_embeds.Count >= _model.Params.BatchSize)
                    {
                        break;
                    }
                }
            }
        }
        public class InstructExecutorState : ExecutorBaseState
        {
            [JsonPropertyName("is_prompt_run")]
            public bool IsPromptRun { get; set; }
            [JsonPropertyName("inp_pfx")]
            public llama_token[] InputPrefixTokens { get; set; }
            [JsonPropertyName("inp_sfx")]
            public llama_token[] InputSuffixTokens { get; set; }
        }
    }
}
