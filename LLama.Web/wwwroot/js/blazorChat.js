window.llamaChat = (() => {
    let markdown = null;
    let audioRecorder = null;
    let audioChunks = [];
    let audioStream = null;
    let audioContext = null;
    let audioSourceNode = null;
    let audioProcessorNode = null;
    let audioSilenceNode = null;
    let audioSampleRate = 0;
    let audioSampleChunks = [];
    let recordedAudioBlob = null;
    let recordedAudioUploadBlob = null;
    let recordedAudioPreviewUrl = null;
    let markdownWarningLogged = false;
    const composerBindings = new WeakMap();

    const escapeHtml = (text) => {
        return (text || "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll("\"", "&quot;")
            .replaceAll("'", "&#39;");
    };

    const createPlainTextRenderer = () => ({
        render: (text) => escapeHtml(text).replaceAll("\n", "<br />")
    });

    const ensureMarkdown = () => {
        if (markdown) {
            return markdown;
        }

        if (typeof window.markdownit !== "function") {
            if (!markdownWarningLogged) {
                console.warn("markdown-it is not available; falling back to plain text rendering.");
                markdownWarningLogged = true;
            }

            markdown = createPlainTextRenderer();
            return markdown;
        }

        markdown = window.markdownit({
            html: false,
            linkify: true,
            breaks: true
        });

        if (window.markdownitTaskLists) {
            markdown.use(window.markdownitTaskLists);
        }
        if (window.markdownitFootnote) {
            markdown.use(window.markdownitFootnote);
        }
        if (window.markdownitDeflist) {
            markdown.use(window.markdownitDeflist);
        }
        if (window.markdownitSub) {
            markdown.use(window.markdownitSub);
        }
        if (window.markdownitSup) {
            markdown.use(window.markdownitSup);
        }
        if (window.markdownitMark) {
            markdown.use(window.markdownitMark);
        }
        if (window.markdownitEmoji) {
            markdown.use(window.markdownitEmoji);
        }

        if (window.mermaid) {
            window.mermaid.initialize({ startOnLoad: false, theme: "dark" });
        }

        return markdown;
    };

    const renderMermaid = (container) => {
        if (!window.mermaid || !container) {
            return;
        }

        const codeBlocks = container.querySelectorAll("pre > code.language-mermaid, pre > code.lang-mermaid, code.language-mermaid");
        if (!codeBlocks.length) {
            return;
        }

        codeBlocks.forEach((code) => {
            const parent = code.parentElement;
            const mermaidDiv = document.createElement("div");
            mermaidDiv.className = "mermaid";
            mermaidDiv.textContent = code.textContent || "";
            parent.replaceWith(mermaidDiv);
        });

        window.mermaid.run({ nodes: container.querySelectorAll('.mermaid') });
    };

    const isAudioRecordingSupported = () => {
        return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia && (window.AudioContext || window.webkitAudioContext || window.MediaRecorder));
    };

    const getSupportedAudioMimeType = () => {
        if (!window.MediaRecorder || !MediaRecorder.isTypeSupported) {
            return "";
        }

        const candidates = [
            "audio/mp4",
            "audio/webm;codecs=opus",
            "audio/webm",
            "audio/ogg;codecs=opus",
            "audio/ogg",
            "audio/mpeg"
        ];

        return candidates.find((candidate) => MediaRecorder.isTypeSupported(candidate)) || "";
    };

    const canPlayAudioType = (mimeType) => {
        if (!mimeType) {
            return false;
        }

        const audio = document.createElement("audio");
        return !!audio.canPlayType && audio.canPlayType(mimeType) !== "";
    };

    const cleanupAudioStream = () => {
        if (audioStream) {
            audioStream.getTracks().forEach((track) => track.stop());
            audioStream = null;
        }
    };

    const cleanupAudioGraph = async () => {
        if (audioProcessorNode) {
            audioProcessorNode.disconnect();
            audioProcessorNode.onaudioprocess = null;
            audioProcessorNode = null;
        }

        if (audioSourceNode) {
            audioSourceNode.disconnect();
            audioSourceNode = null;
        }

        if (audioSilenceNode) {
            audioSilenceNode.disconnect();
            audioSilenceNode = null;
        }

        if (audioContext) {
            try {
                await audioContext.close();
            } catch (err) {
                console.warn("Failed to close audio context.", err);
            }
            audioContext = null;
        }

        audioSampleRate = 0;
        audioSampleChunks = [];
    };

    const clearRecordedAudio = () => {
        recordedAudioBlob = null;
        recordedAudioUploadBlob = null;
        if (recordedAudioPreviewUrl) {
            URL.revokeObjectURL(recordedAudioPreviewUrl);
            recordedAudioPreviewUrl = null;
        }
    };

    const interleaveAudioBuffer = (audioBuffer) => {
        const channelCount = audioBuffer.numberOfChannels;
        const sampleCount = audioBuffer.length;
        const interleaved = new Float32Array(sampleCount * channelCount);

        for (let sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++) {
            for (let channelIndex = 0; channelIndex < channelCount; channelIndex++) {
                interleaved[sampleIndex * channelCount + channelIndex] = audioBuffer.getChannelData(channelIndex)[sampleIndex];
            }
        }

        return interleaved;
    };

    const encodeWav = (audioBuffer) => {
        const channelCount = audioBuffer.numberOfChannels;
        const sampleRate = audioBuffer.sampleRate;
        const samples = interleaveAudioBuffer(audioBuffer);
        const bytesPerSample = 2;
        const blockAlign = channelCount * bytesPerSample;
        const buffer = new ArrayBuffer(44 + samples.length * bytesPerSample);
        const view = new DataView(buffer);

        const writeString = (offset, value) => {
            for (let i = 0; i < value.length; i++) {
                view.setUint8(offset + i, value.charCodeAt(i));
            }
        };

        writeString(0, "RIFF");
        view.setUint32(4, 36 + samples.length * bytesPerSample, true);
        writeString(8, "WAVE");
        writeString(12, "fmt ");
        view.setUint32(16, 16, true);
        view.setUint16(20, 1, true);
        view.setUint16(22, channelCount, true);
        view.setUint32(24, sampleRate, true);
        view.setUint32(28, sampleRate * blockAlign, true);
        view.setUint16(32, blockAlign, true);
        view.setUint16(34, 16, true);
        writeString(36, "data");
        view.setUint32(40, samples.length * bytesPerSample, true);

        let offset = 44;
        for (let i = 0; i < samples.length; i++, offset += 2) {
            const sample = Math.max(-1, Math.min(1, samples[i]));
            view.setInt16(offset, sample < 0 ? sample * 0x8000 : sample * 0x7fff, true);
        }

        return buffer;
    };

    const encodeMonoWav = (sampleRate, samples) => {
        const bytesPerSample = 2;
        const blockAlign = bytesPerSample;
        const buffer = new ArrayBuffer(44 + samples.length * bytesPerSample);
        const view = new DataView(buffer);

        const writeString = (offset, value) => {
            for (let i = 0; i < value.length; i++) {
                view.setUint8(offset + i, value.charCodeAt(i));
            }
        };

        writeString(0, "RIFF");
        view.setUint32(4, 36 + samples.length * bytesPerSample, true);
        writeString(8, "WAVE");
        writeString(12, "fmt ");
        view.setUint32(16, 16, true);
        view.setUint16(20, 1, true);
        view.setUint16(22, 1, true);
        view.setUint32(24, sampleRate, true);
        view.setUint32(28, sampleRate * blockAlign, true);
        view.setUint16(32, blockAlign, true);
        view.setUint16(34, 16, true);
        writeString(36, "data");
        view.setUint32(40, samples.length * bytesPerSample, true);

        let offset = 44;
        for (let i = 0; i < samples.length; i++, offset += 2) {
            const sample = Math.max(-1, Math.min(1, samples[i]));
            view.setInt16(offset, sample < 0 ? sample * 0x8000 : sample * 0x7fff, true);
        }

        return buffer;
    };

    const mergeSampleChunks = (chunks) => {
        const totalLength = chunks.reduce((sum, chunk) => sum + chunk.length, 0);
        const merged = new Float32Array(totalLength);
        let offset = 0;

        for (const chunk of chunks) {
            merged.set(chunk, offset);
            offset += chunk.length;
        }

        return merged;
    };

    const normalizeMonoSamples = (samples) => {
        if (!samples || samples.length === 0) {
            return samples;
        }

        let peak = 0;
        for (let i = 0; i < samples.length; i++) {
            const amplitude = Math.abs(samples[i]);
            if (amplitude > peak) {
                peak = amplitude;
            }
        }

        // Treat near-silence as silence instead of amplifying background noise.
        if (peak < 0.01) {
            return samples;
        }

        const targetPeak = 0.85;
        const maxGain = 8.0;
        const gain = Math.min(maxGain, targetPeak / peak);
        if (gain <= 1.05) {
            return samples;
        }

        const normalized = new Float32Array(samples.length);
        for (let i = 0; i < samples.length; i++) {
            const value = samples[i] * gain;
            normalized[i] = Math.max(-1, Math.min(1, value));
        }

        return normalized;
    };

    const mixInputToMono = (inputBuffer) => {
        const channelCount = inputBuffer.numberOfChannels;
        const sampleCount = inputBuffer.length;
        const mono = new Float32Array(sampleCount);

        for (let channelIndex = 0; channelIndex < channelCount; channelIndex++) {
            const channel = inputBuffer.getChannelData(channelIndex);
            for (let sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++) {
                mono[sampleIndex] += channel[sampleIndex];
            }
        }

        if (channelCount > 1) {
            for (let sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++) {
                mono[sampleIndex] /= channelCount;
            }
        }

        return mono;
    };

    const convertBlobToWav = async (blob) => {
        const AudioContextCtor = window.AudioContext || window.webkitAudioContext;
        if (!AudioContextCtor) {
            return null;
        }

        const audioContext = new AudioContextCtor();
        try {
            const sourceBuffer = await blob.arrayBuffer();
            const decodedBuffer = await audioContext.decodeAudioData(sourceBuffer.slice(0));
            const wavBuffer = encodeWav(decodedBuffer);
            return new Blob([wavBuffer], { type: "audio/wav" });
        } catch (err) {
            console.warn("Failed to normalize recorded audio to WAV.", err);
            return null;
        } finally {
            if (typeof audioContext.close === "function") {
                await audioContext.close();
            }
        }
    };

    return {
        initialize: () => {
            ensureMarkdown();
        },
        renderMarkdown: (element, text) => {
            if (!element) {
                return;
            }

            const md = ensureMarkdown();
            element.innerHTML = md.render(text || "");
            renderMermaid(element);
        },
        scrollToBottom: (element) => {
            if (!element) {
                return;
            }
            element.scrollTop = element.scrollHeight;
        },
        getSidebarTab: () => {
            return window.localStorage.getItem("llama.sidebar.tab");
        },
        setSidebarTab: (value) => {
            if (!value) {
                return;
            }
            window.localStorage.setItem("llama.sidebar.tab", value);
        },
        bindComposerShortcuts: (textarea, sendButton) => {
            if (!textarea || !sendButton) {
                return;
            }

            const existing = composerBindings.get(textarea);
            if (existing === sendButton) {
                return;
            }

            if (!composerBindings.has(textarea)) {
                textarea.addEventListener("keydown", (event) => {
                    if (event.key !== "Enter" || event.isComposing) {
                        return;
                    }

                    if (event.shiftKey || event.ctrlKey || event.altKey || event.metaKey) {
                        return;
                    }

                    event.preventDefault();
                    const targetButton = composerBindings.get(textarea);
                    if (!targetButton || targetButton.disabled) {
                        return;
                    }

                    targetButton.click();
                });
            }

            composerBindings.set(textarea, sendButton);
        },
        isAudioRecordingSupported: () => {
            return isAudioRecordingSupported();
        },
        startAudioRecording: async () => {
            if (!isAudioRecordingSupported()) {
                return { started: false, error: "Audio recording is not supported in this browser." };
            }

            if ((audioRecorder && audioRecorder.state === "recording") || audioProcessorNode) {
                return { started: false, error: "Audio recording is already in progress." };
            }

            try {
                audioStream = await navigator.mediaDevices.getUserMedia({
                    audio: {
                        channelCount: { ideal: 1 },
                        sampleRate: { ideal: 16000 },
                        sampleSize: { ideal: 16 },
                        echoCancellation: false,
                        noiseSuppression: false,
                        autoGainControl: false
                    }
                });
                const AudioContextCtor = window.AudioContext || window.webkitAudioContext;
                if (AudioContextCtor) {
                    audioContext = new AudioContextCtor();
                    if (audioContext.state === "suspended" && typeof audioContext.resume === "function") {
                        await audioContext.resume();
                    }
                    audioSampleRate = audioContext.sampleRate;
                    audioSampleChunks = [];
                    audioSourceNode = audioContext.createMediaStreamSource(audioStream);
                    audioProcessorNode = audioContext.createScriptProcessor(4096, 1, 1);
                    audioSilenceNode = audioContext.createGain();
                    audioSilenceNode.gain.value = 0;

                    audioProcessorNode.onaudioprocess = (event) => {
                        audioSampleChunks.push(mixInputToMono(event.inputBuffer));
                    };

                    audioSourceNode.connect(audioProcessorNode);
                    audioProcessorNode.connect(audioSilenceNode);
                    audioSilenceNode.connect(audioContext.destination);

                    return { started: true, mimeType: "audio/wav" };
                }

                const mimeType = getSupportedAudioMimeType();
                const options = mimeType ? { mimeType } : undefined;
                audioRecorder = new MediaRecorder(audioStream, options);
                audioChunks = [];

                audioRecorder.ondataavailable = (event) => {
                    if (event.data && event.data.size > 0) {
                        audioChunks.push(event.data);
                    }
                };

                audioRecorder.start(250);
                return { started: true, mimeType: audioRecorder.mimeType || mimeType || "" };
            } catch (err) {
                cleanupAudioStream();
                return { started: false, error: err?.message || "Unable to access the microphone." };
            }
        },
        stopAudioRecording: () => {
            if (audioProcessorNode) {
                return (async () => {
                    const mergedSamples = mergeSampleChunks(audioSampleChunks);
                    if (mergedSamples.length === 0) {
                        await cleanupAudioGraph();
                        cleanupAudioStream();
                        return { previewUrl: "", mimeType: "", size: 0 };
                    }

                    const normalizedSamples = normalizeMonoSamples(mergedSamples);
                    const wavBuffer = encodeMonoWav(audioSampleRate || 16000, normalizedSamples);
                    const wavBlob = new Blob([wavBuffer], { type: "audio/wav" });

                    clearRecordedAudio();
                    recordedAudioBlob = wavBlob;
                    recordedAudioUploadBlob = wavBlob;
                    recordedAudioPreviewUrl = URL.createObjectURL(wavBlob);

                    await cleanupAudioGraph();
                    cleanupAudioStream();

                    return { previewUrl: recordedAudioPreviewUrl, mimeType: "audio/wav", size: wavBlob.size };
                })();
            }

            if (!audioRecorder || audioRecorder.state !== "recording") {
                cleanupAudioGraph();
                cleanupAudioStream();
                audioRecorder = null;
                audioChunks = [];
                return Promise.resolve({ previewUrl: "", mimeType: "", size: 0 });
            }

            return new Promise((resolve) => {
                audioRecorder.onstop = async () => {
                    const blobType = audioRecorder.mimeType || "audio/webm";
                    const rawBlob = new Blob(audioChunks, { type: blobType });
                    const uploadBlob = await convertBlobToWav(rawBlob);
                    if (!uploadBlob || uploadBlob.size === 0 || uploadBlob.type !== "audio/wav") {
                        clearRecordedAudio();
                        cleanupAudioStream();
                        audioRecorder = null;
                        audioChunks = [];
                        resolve({
                            previewUrl: "",
                            mimeType: "",
                            size: 0,
                            error: "Unable to convert the recording to WAV for upload."
                        });
                        return;
                    }

                    if (uploadBlob.size === 0) {
                        clearRecordedAudio();
                        cleanupAudioStream();
                        audioRecorder = null;
                        audioChunks = [];
                        resolve({ previewUrl: "", mimeType: "", size: 0 });
                        return;
                    }

                    clearRecordedAudio();
                    recordedAudioBlob = uploadBlob;
                    recordedAudioUploadBlob = uploadBlob;
                    recordedAudioPreviewUrl = URL.createObjectURL(uploadBlob);
                    cleanupAudioStream();
                    audioRecorder = null;
                    audioChunks = [];
                    resolve({ previewUrl: recordedAudioPreviewUrl, mimeType: uploadBlob.type, size: uploadBlob.size });
                };

                try {
                    audioRecorder.requestData();
                } catch (err) {
                    console.warn("Failed to flush recorded audio before stopping.", err);
                }

                audioRecorder.stop();
            });
        },
        discardRecordedAudio: () => {
            clearRecordedAudio();
        },
        uploadRecordedAudio: async (connectionId, fileName) => {
            const uploadBlob = recordedAudioUploadBlob || recordedAudioBlob;
            if (!uploadBlob) {
                return { attachments: [] };
            }

            const uploadFileName = fileName || "recording.wav";
            const formData = new FormData();
            formData.append("connectionId", connectionId);
            formData.append("files", uploadBlob, uploadFileName);

            const response = await fetch("/api/attachments", {
                method: "POST",
                body: formData
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Unable to upload recorded audio.");
            }

            const result = await response.json();
            clearRecordedAudio();
            return result;
        }
    };
})();
