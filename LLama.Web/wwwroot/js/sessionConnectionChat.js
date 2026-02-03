const createConnectionSessionChat = () => {
    const outputErrorTemplate = $("#outputErrorTemplate").html();
    const outputInfoTemplate = $("#outputInfoTemplate").html();
    const outputUserTemplate = $("#outputUserTemplate").html();
    const outputBotTemplate = $("#outputBotTemplate").html();
    const signatureTemplate = $("#signatureTemplate").html();

    let inferenceSession;
    let connectionId;
    const connection = new signalR.HubConnectionBuilder().withUrl("/SessionConnectionHub").build();

    const scrollContainer = $("#scroll-container");
    const outputContainer = $("#output-container");
    const chatInput = $("#input");
    const modelSelect = $("#model-select");
    const downloadList = $("#download-list");
    const downloadEmpty = $("#download-empty");
    const attachmentInput = $("#attachments");
    const attachmentList = $("#attachment-list");
    const clearAttachmentsButton = $("#clear-attachments");
    const modelsPath = $("#models-path");
    const downloadsPath = $("#downloads-path");
    const uploadsPath = $("#uploads-path");

    const downloadState = {};
    let pendingAttachments = [];

    const renderQueue = new Map();
    let renderScheduled = false;

    const onStatus = (id, status) => {
        connectionId = id;
        if (status == Enums.SessionConnectionStatus.Disconnected) {
            onError("Socket not connected")
        }
        else if (status == Enums.SessionConnectionStatus.Connected) {
            onInfo("Socket connected")
        }
        else if (status == Enums.SessionConnectionStatus.Loaded) {
            loaderHide();
            enableControls();
            $("#load").hide();
            $("#unload").show();
        }
    }

    const onError = (error) => {
        enableControls();
        outputContainer.append(Mustache.render(outputErrorTemplate, { text: error, date: getDateTime() }));
    }

    const onInfo = (message) => {
        outputContainer.append(Mustache.render(outputInfoTemplate, { text: message, date: getDateTime() }));
    }

    const onStorageInfo = (info) => {
        if (!info)
            return;

        modelsPath.text(info.modelsPath || "-");
        downloadsPath.text(info.downloadsPath || "-");
        uploadsPath.text(info.uploadsPath || "-");
    }

    let responseContent;
    let responseContainer;
    let responseRaw = "";

    const onResponse = (response) => {
        if (!response)
            return;

        if (response.tokenType == Enums.TokenType.Begin) {
            let uniqueId = randomString();
            outputContainer.append(Mustache.render(outputBotTemplate, { uniqueId: uniqueId, ...response }));
            responseContainer = $(`#${uniqueId}`);
            responseContent = responseContainer.find(".content");
            responseRaw = "";
            responseContent.text("...");
            scrollToBottom(true);
            return;
        }

        if (response.tokenType == Enums.TokenType.End || response.tokenType == Enums.TokenType.Cancel) {
            enableControls();
            responseContainer.find(".signature").append(Mustache.render(signatureTemplate, response));
            scrollToBottom();
        }
        else {
            if (responseContent) {
                if (responseRaw.length === 0) {
                    responseContainer.find(".date").append(getDateTime());
                }
                responseRaw += response.content;
                scheduleRender(responseContent, responseRaw);
                scrollToBottom();
            }
        }
    }

    const sendPrompt = async () => {
        const text = chatInput.val();
        if (!text)
            return;

        if (!isSelectedModelReady()) {
            onError("Selected model is still downloading.");
            return;
        }

        chatInput.val(null);
        disableControls();

        let attachments = [];
        if (pendingAttachments.length > 0) {
            try {
                attachments = await uploadAttachments();
            } catch (error) {
                onError(error.message || "Failed to upload attachments.");
                enableControls();
                return;
            }
        }

        outputContainer.append(Mustache.render(outputUserTemplate, { text: text, date: getDateTime(), attachments: attachments }));
        renderMarkdownAsync(outputContainer.find(".entry:last .content"), text);

        const request = { prompt: text, attachmentIds: attachments.map(a => a.id) };
        inferenceSession = await connection
            .stream("SendPrompt", request, serializeFormToJson('SessionParameters'))
            .subscribe({
                next: onResponse,
                complete: onResponse,
                error: onError,
            });
        scrollToBottom(true);
    }

    const cancelPrompt = async () => {
        if (inferenceSession)
            inferenceSession.dispose();
    }

    const loadModel = async () => {
        if (!isSelectedModelReady()) {
            onError("Selected model is still downloading.");
            return;
        }

        const sessionParams = serializeFormToJson('SessionParameters');
        loaderShow();
        disableControls();
        disablePromptControls();
        $("#load").attr("disabled", "disabled");

        // TODO: Split parameters sets
        await connection.invoke('LoadModel', sessionParams, sessionParams);
    }

    const unloadModel = async () => {
        await cancelPrompt();
        disableControls();
        enablePromptControls();
        $("#load").removeAttr("disabled");
    }

    const uploadAttachments = async () => {
        if (pendingAttachments.length === 0)
            return [];

        if (!connectionId)
            throw new Error("Connection not ready for uploads.");

        const formData = new FormData();
        formData.append("connectionId", connectionId);
        pendingAttachments.forEach(file => formData.append("files", file));

        onInfo("Uploading attachments...");
        const response = await fetch("/api/attachments", { method: "POST", body: formData });
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Attachment upload failed.");
        }

        const result = await response.json();
        pendingAttachments = [];
        attachmentInput.val("");
        renderAttachmentList();
        return result.attachments || [];
    }

    const serializeFormToJson = (form) => {
        const formDataJson = {};
        const formData = new FormData(document.getElementById(form));
        formData.forEach((value, key) => {

            if (key.includes("."))
                key = key.split(".")[1];

            // Convert number strings to numbers
            if (!isNaN(value) && value.trim() !== "") {
                formDataJson[key] = parseFloat(value);
            }
            // Convert boolean strings to booleans
            else if (value === "true" || value === "false") {
                formDataJson[key] = (value === "true");
            }
            else {
                formDataJson[key] = value;
            }
        });
        return formDataJson;
    }

    const enableControls = () => {
        $(".input-control").removeAttr("disabled");
    }

    const disableControls = () => {
        $(".input-control").attr("disabled", "disabled");
    }

    const enablePromptControls = () => {
        $("#load").show();
        $("#unload").hide();
        $(".prompt-control").removeAttr("disabled");
        activatePromptTab();
        updateLoadButtonState();
    }

    const disablePromptControls = () => {
        $(".prompt-control").attr("disabled", "disabled");
        activateParamsTab();
    }

    const clearOutput = () => {
        outputContainer.empty();
    }

    const updatePrompt = () => {
        const customPrompt = $("#PromptText");
        const selection = $("option:selected", "#Prompt");
        const selectedValue = selection.data("prompt");
        customPrompt.text(selectedValue);
    }

    const getDateTime = () => {
        const dateTime = new Date();
        return dateTime.toLocaleString();
    }

    const randomString = () => {
        return Math.random().toString(36).slice(2);
    }

    const scrollToBottom = (force) => {
        const scrollTop = scrollContainer.scrollTop();
        const scrollHeight = scrollContainer[0].scrollHeight;
        if (force) {
            scrollContainer.scrollTop(scrollContainer[0].scrollHeight);
            return;
        }
        if (scrollTop + 70 >= scrollHeight - scrollContainer.innerHeight()) {
            scrollContainer.scrollTop(scrollContainer[0].scrollHeight)
        }
    }

    const activatePromptTab = () => {
        $("#nav-prompt-tab").trigger("click");
    }

    const activateParamsTab = () => {
        $("#nav-params-tab").trigger("click");
    }

    const loaderShow = () => {
        $(".spinner").show();
    }

    const loaderHide = () => {
        $(".spinner").hide();
    }

    const createMarkdownRenderer = () => {
        if (typeof window.markdownit === "undefined")
            return null;

        const md = window.markdownit({
            html: false,
            linkify: true,
            breaks: true,
            typographer: true
        });

        if (window.markdownitKatex)
            md.use(window.markdownitKatex);
        if (window.markdownitTaskLists)
            md.use(window.markdownitTaskLists, { enabled: true, label: true });
        if (window.markdownitFootnote)
            md.use(window.markdownitFootnote);
        if (window.markdownitDeflist)
            md.use(window.markdownitDeflist);
        if (window.markdownitSub)
            md.use(window.markdownitSub);
        if (window.markdownitSup)
            md.use(window.markdownitSup);
        if (window.markdownitMark)
            md.use(window.markdownitMark);
        if (window.markdownitEmoji)
            md.use(window.markdownitEmoji);

        if (window.mermaid) {
            window.mermaid.initialize({ startOnLoad: false });
        }

        return md;
    }

    const markdown = createMarkdownRenderer();

    const scheduleRender = (container, raw) => {
        if (!markdown) {
            container.text(raw);
            return;
        }

        renderQueue.set(container[0], { container, raw });
        if (renderScheduled)
            return;

        renderScheduled = true;
        requestAnimationFrame(() => {
            renderScheduled = false;
            renderQueue.forEach((value) => {
                renderMarkdown(value.container, value.raw);
            });
            renderQueue.clear();
        });
    }

    const renderMarkdownAsync = (container, raw) => {
        scheduleRender(container, raw);
    }

    const renderMarkdown = (container, raw) => {
        try {
            container.html(markdown.render(raw || ""));
            renderMermaid(container);
        } catch (error) {
            container.text(raw);
        }
    }

    const renderMermaid = (container) => {
        if (!window.mermaid)
            return;

        const blocks = container.find("pre code.language-mermaid");
        if (!blocks.length)
            return;

        blocks.each((index, block) => {
            const code = $(block);
            const parent = code.parent();
            const mermaidDiv = $("<div class=\"mermaid\"></div>");
            mermaidDiv.text(code.text());
            parent.replaceWith(mermaidDiv);
        });

        window.mermaid.run({ nodes: container[0].querySelectorAll('.mermaid') });
    }

    const renderAttachmentList = () => {
        attachmentList.empty();
        if (pendingAttachments.length === 0)
            return;

        pendingAttachments.forEach(file => {
            attachmentList.append(`<div class=\"attachment-pill\">${file.name}</div>`);
        });
    }

    const formatBytes = (value) => {
        if (value === null || value === undefined)
            return "";
        if (value === 0)
            return "0 B";

        const k = 1024;
        const sizes = ["B", "KB", "MB", "GB", "TB"];
        const i = Math.floor(Math.log(value) / Math.log(k));
        return `${(value / Math.pow(k, i)).toFixed(1)} ${sizes[i]}`;
    }

    const updateDownloadState = (progress) => {
        if (!progress || !progress.modelName)
            return;

        if (!downloadState[progress.modelName])
            downloadState[progress.modelName] = { assets: {}, ready: false };

        const assetKey = `${progress.assetKind}-${progress.fileName}`;
        downloadState[progress.modelName].assets[assetKey] = progress;
        refreshDownloadUI();
    }

    const applyDownloadSnapshot = (snapshots) => {
        if (!snapshots)
            return;

        snapshots.forEach(snapshot => {
            downloadState[snapshot.modelName] = { assets: {}, ready: snapshot.ready };
            snapshot.assets.forEach(asset => {
                const assetKey = `${asset.assetKind}-${asset.fileName}`;
                downloadState[snapshot.modelName].assets[assetKey] = asset;
            });
        });

        refreshDownloadUI();
    }

    const refreshDownloadUI = () => {
        downloadList.empty();
        let anyActive = false;

        Object.keys(downloadState).forEach(modelName => {
            const modelEntry = downloadState[modelName];
            const assets = Object.values(modelEntry.assets);
            if (assets.length === 0)
                return;

            const ready = assets.every(asset => asset.state === 2);
            modelEntry.ready = ready;

            const totalBytes = assets.reduce((sum, asset) => sum + (asset.totalBytes || 0), 0);
            const receivedBytes = assets.reduce((sum, asset) => sum + (asset.bytesReceived || 0), 0);
            const hasUnknownTotal = assets.some(asset => asset.totalBytes === null || asset.totalBytes === undefined);
            const hasFailure = assets.some(asset => asset.state === 3 || asset.state === 4);
            const isDownloading = assets.some(asset => asset.state === 1);

            const statusText = ready
                ? "Ready"
                : hasFailure
                    ? "Error"
                    : isDownloading
                        ? "Downloading"
                        : "Queued";

            const progressPercent = totalBytes > 0 ? Math.min(100, Math.round((receivedBytes / totalBytes) * 100)) : 0;
            const remainingText = hasUnknownTotal
                ? "Calculating size..."
                : `${formatBytes(Math.max(0, totalBytes - receivedBytes))} remaining`;

            if (!ready)
                anyActive = true;

            const assetRows = assets.map(asset => {
                const assetTotal = asset.totalBytes || 0;
                const assetPercent = assetTotal > 0 ? Math.min(100, Math.round((asset.bytesReceived / assetTotal) * 100)) : 0;
                const assetStatus = asset.state === 2
                    ? "Ready"
                    : asset.state === 3
                        ? "Failed"
                        : asset.state === 4
                            ? "Missing URL"
                            : asset.state === 1
                                ? "Downloading"
                                : "Queued";
                const assetMeta = assetTotal > 0
                    ? `${formatBytes(asset.bytesReceived)} / ${formatBytes(assetTotal)}`
                    : asset.bytesReceived > 0
                        ? formatBytes(asset.bytesReceived)
                        : "-";

                return `<div class=\"download-asset\">
                            <div class=\"d-flex justify-content-between\">
                                <div class=\"download-asset-name\">${asset.fileName}</div>
                                <div class=\"download-asset-status\">${assetStatus}</div>
                            </div>
                            <div class=\"progress progress-sm\">
                                <div class=\"progress-bar\" role=\"progressbar\" style=\"width: ${assetPercent}%\"></div>
                            </div>
                            <div class=\"download-asset-meta\">${assetMeta}</div>
                        </div>`;
            }).join("");

            const item = $(
                `<div class=\"download-item\" data-model=\"${modelName}\">
                    <div class=\"d-flex justify-content-between\">
                        <div class=\"download-name\">${modelName}</div>
                        <div class=\"download-status\">${statusText}</div>
                    </div>
                    <div class=\"progress\">
                        <div class=\"progress-bar\" role=\"progressbar\" style=\"width: ${progressPercent}%\" aria-valuenow=\"${progressPercent}\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>
                    </div>
                    <div class=\"download-meta\">${remainingText}</div>
                    <div class=\"download-asset-list\">${assetRows}</div>
                </div>`
            );

            downloadList.append(item);
            updateModelOptionState(modelName, ready);
        });

        downloadEmpty.toggle(!anyActive);
        updateLoadButtonState();
    }

    const updateModelOptionState = (modelName, ready) => {
        const option = modelSelect.find(`option[value='${modelName}']`);
        option.prop("disabled", !ready);
    }

    const isSelectedModelReady = () => {
        const selected = modelSelect.val();
        if (!selected)
            return false;

        const entry = downloadState[selected];
        return entry ? entry.ready : false;
    }

    const updateLoadButtonState = () => {
        const ready = isSelectedModelReady();
        if (ready) {
            $("#load").removeAttr("disabled");
        } else {
            $("#load").attr("disabled", "disabled");
        }
    }

    // Map UI functions
    $("#load").on("click", loadModel);
    $("#unload").on("click", unloadModel);
    $("#send").on("click", sendPrompt);
    $("#clear").on("click", clearOutput);
    $("#cancel").on("click", cancelPrompt);
    $("#Prompt").on("change", updatePrompt);
    modelSelect.on("change", updateLoadButtonState);
    attachmentInput.on("change", () => {
        pendingAttachments = Array.from(attachmentInput[0].files || []);
        renderAttachmentList();
    });
    clearAttachmentsButton.on("click", () => {
        pendingAttachments = [];
        attachmentInput.val("");
        renderAttachmentList();
    });
    chatInput.on('keydown', function (event) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            sendPrompt();
        }
    });
    $(".slider").on("input", function (e) {
        const slider = $(this);
        slider.next().text(slider.val());
    }).trigger("input");


    // Map signalr functions
    connection.on("OnStatus", onStatus);
    connection.on("OnError", onError);
    connection.on("OnResponse", onResponse);
    connection.on("OnModelDownloadSnapshot", applyDownloadSnapshot);
    connection.on("OnModelDownloadProgress", updateDownloadState);
    connection.on("OnStorageInfo", onStorageInfo);
    connection.start().then(() => {
        connection.invoke("GetModelStatuses").then(applyDownloadSnapshot);
        setInterval(() => {
            connection.invoke("GetModelStatuses").then(applyDownloadSnapshot);
        }, 2000);
    });
}
