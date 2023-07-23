const createConnectionSessionChat = (LLamaExecutorType) => {
    const outputErrorTemplate = $("#outputErrorTemplate").html();
    const outputInfoTemplate = $("#outputInfoTemplate").html();
    const outputUserTemplate = $("#outputUserTemplate").html();
    const outputBotTemplate = $("#outputBotTemplate").html();
    const sessionDetailsTemplate = $("#sessionDetailsTemplate").html();

    let connectionId;
    const connection = new signalR.HubConnectionBuilder().withUrl("/SessionConnectionHub").build();

    const scrollContainer = $("#scroll-container");
    const outputContainer = $("#output-container");
    const chatInput = $("#input");


    const onStatus = (connection, status) => {
        connectionId = connection;
        if (status == Enums.SessionConnectionStatus.Connected) {
            $("#socket").text("Connected").addClass("text-success");
        }
        else if (status == Enums.SessionConnectionStatus.Loaded) {
            enableControls();
            $("#session-details").html(Mustache.render(sessionDetailsTemplate, { model: getSelectedModel(), prompt: getSelectedPrompt(), parameter: getSelectedParameter() }));
            onInfo(`New model session successfully started`)
        }
    }

    const onError = (error) => {
        enableControls();
        outputContainer.append(Mustache.render(outputErrorTemplate, { text: error, date: getDateTime() }));
    }

    const onInfo = (message) => {
        outputContainer.append(Mustache.render(outputInfoTemplate, { text: message, date: getDateTime() }));
    }

    let responseContent;
    let responseContainer;
    let responseFirstFragment;

    const onResponse = (response) => {
        if (!response)
            return;

        if (response.isFirst) {
            outputContainer.append(Mustache.render(outputBotTemplate, response));
            responseContainer = $(`#${response.id}`);
            responseContent = responseContainer.find(".content");
            responseFirstFragment = true;
            scrollToBottom(true);
            return;
        }

        if (response.isLast) {
            enableControls();
            responseContainer.find(".signature").append(response.content);
            scrollToBottom();
        }
        else {
            if (responseFirstFragment) {
                responseContent.empty();
                responseFirstFragment = false;
                responseContainer.find(".date").append(getDateTime());
            }
            responseContent.append(response.content);
            scrollToBottom();
        }
    }


    const sendPrompt = async () => {
        const text = chatInput.val();
        if (text) {
            disableControls();
            outputContainer.append(Mustache.render(outputUserTemplate, { text: text, date: getDateTime() }));
            await connection.invoke('SendPrompt', text);
            chatInput.val(null);
            scrollToBottom(true);
        }
    }

    const cancelPrompt = async () => {
        await ajaxPostJsonAsync('?handler=Cancel', { connectionId: connectionId });
    }

    const loadModel = async () => {
        const modelName = getSelectedModel();
        const promptName = getSelectedPrompt();
        const parameterName = getSelectedParameter();
        if (!modelName || !promptName || !parameterName) {
            onError("Please select a valid Model, Parameter and Prompt");
            return;
        }

        disableControls();
        await connection.invoke('LoadModel', LLamaExecutorType, modelName, promptName, parameterName);
    }


    const enableControls = () => {
        $(".input-control").removeAttr("disabled");
    }


    const disableControls = () => {
        $(".input-control").attr("disabled", "disabled");
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


    const getSelectedModel = () => {
        return $("option:selected", "#Model").val();
    }


    const getSelectedParameter = () => {
        return $("option:selected", "#Parameter").val();
    }


    const getSelectedPrompt = () => {
        return $("option:selected", "#Prompt").val();
    }


    const getDateTime = () => {
        const dateTime = new Date();
        return dateTime.toLocaleString();
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



    // Map UI functions
    $("#load").on("click", loadModel);
    $("#send").on("click", sendPrompt);
    $("#clear").on("click", clearOutput);
    $("#cancel").on("click", cancelPrompt);
    $("#Prompt").on("change", updatePrompt);
    chatInput.on('keydown', function (event) {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            sendPrompt();
        }
    });



    // Map signalr functions
    connection.on("OnStatus", onStatus);
    connection.on("OnError", onError);
    connection.on("OnResponse", onResponse);
    connection.start();
}