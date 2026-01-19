window.llamaChat = (() => {
    let markdown = null;
    let audioRecorder = null;
    let audioChunks = [];
    let audioStream = null;

    const ensureMarkdown = () => {
        if (markdown) {
            return markdown;
        }

        markdown = window.markdownit({
            html: false,
            linkify: true,
            breaks: true
        });

        if (window.markdownitKatex) {
            markdown.use(window.markdownitKatex);
        }
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
        return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia && window.MediaRecorder);
    };

    const getSupportedAudioMimeType = () => {
        if (!window.MediaRecorder || !MediaRecorder.isTypeSupported) {
            return "";
        }

        const candidates = [
            "audio/webm;codecs=opus",
            "audio/webm",
            "audio/ogg;codecs=opus",
            "audio/ogg",
            "audio/mp4",
            "audio/mpeg"
        ];

        return candidates.find((candidate) => MediaRecorder.isTypeSupported(candidate)) || "";
    };

    const cleanupAudioStream = () => {
        if (audioStream) {
            audioStream.getTracks().forEach((track) => track.stop());
            audioStream = null;
        }
    };

    const bufferToBase64 = (buffer) => {
        const bytes = new Uint8Array(buffer);
        let binary = "";
        const chunkSize = 0x8000;
        for (let i = 0; i < bytes.length; i += chunkSize) {
            binary += String.fromCharCode.apply(null, bytes.subarray(i, i + chunkSize));
        }
        return btoa(binary);
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
        isAudioRecordingSupported: () => {
            return isAudioRecordingSupported();
        },
        startAudioRecording: async () => {
            if (!isAudioRecordingSupported()) {
                return { started: false, error: "Audio recording is not supported in this browser." };
            }

            if (audioRecorder && audioRecorder.state === "recording") {
                return { started: false, error: "Audio recording is already in progress." };
            }

            try {
                audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
                const mimeType = getSupportedAudioMimeType();
                const options = mimeType ? { mimeType } : undefined;
                audioRecorder = new MediaRecorder(audioStream, options);
                audioChunks = [];

                audioRecorder.ondataavailable = (event) => {
                    if (event.data && event.data.size > 0) {
                        audioChunks.push(event.data);
                    }
                };

                audioRecorder.start();
                return { started: true, mimeType: audioRecorder.mimeType || mimeType || "" };
            } catch (err) {
                cleanupAudioStream();
                return { started: false, error: err?.message || "Unable to access the microphone." };
            }
        },
        stopAudioRecording: () => {
            if (!audioRecorder || audioRecorder.state !== "recording") {
                cleanupAudioStream();
                audioRecorder = null;
                audioChunks = [];
                return Promise.resolve({ base64: "", mimeType: "", size: 0 });
            }

            return new Promise((resolve) => {
                audioRecorder.onstop = async () => {
                    const blobType = audioRecorder.mimeType || "audio/webm";
                    const blob = new Blob(audioChunks, { type: blobType });
                    const buffer = await blob.arrayBuffer();
                    const base64 = bufferToBase64(buffer);
                    cleanupAudioStream();
                    audioRecorder = null;
                    audioChunks = [];
                    resolve({ base64, mimeType: blob.type, size: blob.size });
                };

                audioRecorder.stop();
            });
        }
    };
})();
