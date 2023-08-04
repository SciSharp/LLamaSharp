const createConnectionSessionChat = () => {
    const outputErrorTemplate = $("#outputErrorTemplate").html();
    const outputInfoTemplate = $("#outputInfoTemplate").html();
    const outputUserTemplate = $("#outputUserTemplate").html();
    const outputBotTemplate = $("#outputBotTemplate").html();
    const signatureTemplate = $("#signatureTemplate").html();

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
            responseContainer.find(".signature").append(Mustache.render(signatureTemplate, response));
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
            chatInput.val(null);
            disableControls();
            outputContainer.append(Mustache.render(outputUserTemplate, { text: text, date: getDateTime() }));
            await connection.invoke('SendPrompt', text);
            scrollToBottom(true);
        }
    }

    const cancelPrompt = async () => {
        await ajaxPostJsonAsync('?handler=Cancel', { connectionId: connectionId });
    }

    const loadModel = async () => {
        disableControls();
        await connection.invoke('LoadModel', serializeFormToJson('SessionParameters'));
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
    $(".slider").on("input", function (e) {
        const slider = $(this);
        slider.next().text(slider.val());
    }).trigger("input");


    // Map signalr functions
    connection.on("OnStatus", onStatus);
    connection.on("OnError", onError);
    connection.on("OnResponse", onResponse);
    connection.start();
}