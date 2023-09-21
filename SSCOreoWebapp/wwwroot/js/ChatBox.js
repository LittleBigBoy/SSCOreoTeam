const chatInput = document.querySelector(".chat-input textarea");
const sendChatBtn = document.querySelector(".chat-input span");
const chatbox = document.querySelector(".chatbox");
const chatToggler = document.querySelector(".chatbot-toggler");
const chatbotCloseBtn = document.querySelector(".close-btn");

let userMessage;

const createChatLi = (message, className) => {
    const chatli = document.createElement("li");
    chatli.classList.add("chat", className);
    let chatContent = className === "outgoing" ?
        `<p>${message}</p>` : `${message}`;
    chatli.innerHTML = chatContent;
    return chatli;
}
const generateResponse = (incomingChatLi) => {
    //API
    /*const api_url;*/
    const msgElement = incomingChatLi.querySelector("p");
}
const handleChat = () => {
    userMessage = chatInput.value.trim();
    if (!userMessage) return;
   
    chatbox.appendChild(createChatLi(userMessage, "outgoing"));
    chatbox.scrollTo(0, chatbox.scrollHeight);
    
    setTimeout(() => {
        message = getAnswer();
        chatInput.value = "";
    }, 600);
}
document.onkeydown = function (e) {
    var ev = document.all ? window.event : e;
    if (ev.keyCode == 13 && $(".chat-input textarea").is(":focus") && $.trim($(".chat-input textarea").val()) !== "") {
        handleChat()
    }
}

const removeBlank = () => {
    chatInput.value = chatInput.value.replace(/[\r\n]/g, "");
}

const getAnswer = () => {
    query = chatInput.value.trim();
    $.ajax
        ({
            url: "/api/Chat",
            dataType: "json",
            type: "get",
            data: {
                q: query
            },
            success: function (res) {
                console.log(res);
                var data = res.data;
                var htmlStr = '<p>' + data.tittle + '<br />';
                if (data.portfolioNames != null && data.portfolioNames.length > 0) {
                    data.portfolioNames.map((element, index) => {
                        /* бн */
                        htmlStr += '<a class="_chatMessageErrorContent_onjb8_125" data-portfoliotype="' + data.portfolioType + '" data-portfolioname="' + element + '" data-assettype="' + data.assetType + '" data-bs-toggle="modal" data-bs-target="#chatModal" onClick="lineChat(this)">' + (index + 1) + '. ' + element + '</a><br />';

                    })
                }
                htmlStr += '</p>'
                const incomingChatLi = createChatLi(htmlStr, "incoming");
                chatbox.appendChild(incomingChatLi);
                chatbox.scrollTo(0, chatbox.scrollHeight);
                generateResponse(incomingChatLi);
            },
            error: function (ex) {
                alert('failed!');
            },
        });
}

const lineChat = (e) => {
    console.log(e);
    $("#myPortfolioChart").empty();
    $("#myPortfolioChart").append(' <canvas id="portfolioChart" style="max-height: 400px;"></canvas>')
    var dataType = $(e).data("portfoliotype");
    var portfolioName = $(e).data("portfolioname");
    var assetType = $(e).data("assettype");
    $.ajax({
        url: "/api/Portfolio/name/" + portfolioName + "/dataType/" + dataType + "",
        dataType: "json",
        type: "get",
        data: {
        },
        success: function (res) {
            console.log(res);
            var labels = res.map(x => x.asOf);
            var dataData = assetType == "return" ? res.map(x => x.return) : res.map(x => x.nav);
            var title = portfolioName;
            const ctxCom = document.getElementById("portfolioChart");
            const data = {
                labels: labels,
                datasets: [
                    {
                        label: dataType,
                        data: dataData,
                        borderColor: Samples.utils.CHART_COLORS.blue,
                        backgroundColor: Samples.utils.CHART_COLORS.blue,
                        pointStyle: false,
                        cubicInterpolationMode: 'monotone',
                        pointRadius: 10,
                        pointHoverRadius: 15
                    }
                ]
            };
            const config = {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    plugins: {
                        title: {
                            display: true,
                            text: title,
                        }
                    }
                }
            };
            new Chart(ctxCom, config);

        },
        error: function (ex) {
            alert('failed!');
        }
    })
}

chatToggler.addEventListener("click", () => document.body.classList.toggle("show-chatbot"));
chatbotCloseBtn.addEventListener("click", () => document.body.classList.toggle("show-chatbot"));
sendChatBtn.addEventListener("click", handleChat);
chatInput.addEventListener("input", removeBlank);