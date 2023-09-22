initPage()
function initPage() {
    initClient();
    renderChart();
    initServices();
    getPredictedNetIncome();
    getRecommadationService();
}

// Clients
function initClient() {
    $.ajax
        ({
            url: "/api/Client/serviceClients",
            dataType: "json",
            type: "get",
            data: {
            },
            success: function (res) {
                $("#clientlist").empty();
                res.map(x => $("#clientlist").append('<li><a class="dropdown-item" onclick=\'onClientClick("' + x + '")\'>' + x + '</a></li>'));
            },
            error: function (ex) {
                alert('failed!');
            },
        });
}
function renderChart() {
    var client = $("#clientText").html();
    $.ajax
        ({
            url: '/api/Client/' + client + '/services',
            dataType: "json",
            type: "get",
            data: {
            },
            success: function (res) {
                var labels = res.map(x => x.serviceName + "(" + x.percentage + ")");
                var datas = res.map(x => x.amount)
                /// pie
                $("#myPie").empty();
                $("#myPie").append(' <canvas id="pieChart" style="max-height: 400px;"></canvas>')
                const ctx = document.getElementById('pieChart');
                new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: "",
                            data: datas,
                            borderWidth: 1,
                            backgroundColor: Samples.utils.defineColor,
                        }]
                    }
                });
            },
            error: function (ex) {
                alert('failed! exception: ' + ex);
            },
        });
}
function onClientClick(client) {
    var clientText = $("#clientText").html();
    if (client !== clientText) {
        $("#clientText").html(client);
        renderChart();
        getRecommadationService();
        getPredictedNetIncome();
    }
}
function initServices() {
    $.ajax
        ({
            url: '/api/Service',
            dataType: "json",
            type: "get",
            data: {},
            success: function (res) {
                const leftPanel = document.querySelector(".left-panel");
                res.map(x => {
                    var html = `<div class="item row g-2" ondblclick=dblClickPanel(this)>
            <div class="col-auto">
                <label for="${x.replace(/\s*/g, "")}" class="visually-hidden">${x}</label>
                <input type="text" readonly class="form-control-plaintext" id="${x.replace(/\s*/g, "")}" value="${x}">
            </div>
            <div class="col-auto">
                <label for="service1amount" class="visually-hidden">amount</label>
                    <input type="text" class="serviceAmount form-control" data-service="${x}" id="${x.replace(/\s*/g, "")}amount" placeholder="amount">
            </div>
        </div>`
                    $(leftPanel).append(html);
                });
            },
            error: function (ex) {
                alert('failed! exception: ' + ex);
            },
        });
}
function dblClickPanel(e) {
    const leftPanel = document.querySelector(".left-panel");
    const rightPanel = document.querySelector(".right-panel");
    if (e.parentElement === leftPanel) {
        rightPanel.appendChild(e);
    } else {
        leftPanel.appendChild(e);
    }
}

function calculateCustomizeService() {
    const rightPanel = document.querySelector(".right-panel");

    const items = rightPanel.children;
    var data = [];
    for (var i = 0; i < items.length; i++) {
        var serviceName = $(items[i]).find(".serviceAmount").data("service");
        var amount = $(items[i]).find(".serviceAmount").val();
        var item = {
            ServiceName: serviceName,
            Amount: parseFloat(amount),
            Percentage:'',
        }
        data.push(item);
    }
    $.ajax
        ({
            url: '/api/Service/customizeService',
            dataType: "json",
            type: "post",
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify(data) ,
            success: function (res) {
                console.log(res);
                if (res.length == 0) {
                    $("#myCustomizePieChart").empty();
                    $("#myCustomizePieChart").append('<div>Please select some services to generate!</div>')
                    return;
                }
                var labels = res.map(x => x.serviceName + "(" + x.amount + ")");
                var totalAmount = 0
                var datas = res.map(x => totalAmount = totalAmount+ x.amount)
                console.log(totalAmount)
                /// pie
                $("#myCustomizePieChart").empty();
                $("#myCustomizePieChart").append(' <canvas id="customizePieChart" style="max-height: 400px;"></canvas>')
                const ctx = document.getElementById('customizePieChart');
                new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: "",
                            data: datas,
                            borderWidth: 1,
                            backgroundColor: Samples.utils.defineColor,
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                position: 'top',
                            },
                            title: {
                                display: true,
                                text: 'What-if fee'
                            }
                        }
                    }
                });
            },
            error: function (ex) {
                alert('failed! exception: ' + ex);
            },
        });
}

function getPredictedNetIncome() {
    var client = $("#clientText").html();
    $.ajax
        ({
            url: "/api/Client/" + client +"/PredictedNetIncome",
            dataType: "json",
            type: "get",
            data: {
            },
            success: function (res) {
                $("#AnnualFeeRate").html(res.annualFeeRate);
                $("#TotalServiceFee").html(res.totalServiceFee);
                $("#TotalServiceCost").html(res.totalServiceCost);
                $("#ProfitMargin").html(res.profitMargin);
                $("#NetIncome").html(res.netIncome);
                $("#PredictedNetIncome").html(res.predictedNetIncome);
            },
            error: function (ex) {
                alert('failed!');
            },
        });
}

function getRecommadationService() {
    var client = $("#clientText").html();
    $.ajax
        ({
            url: "/api/Service/client/" + client,
            dataType: "json",
            type: "get",
            data: {
            },
            success: function (res) {
                $("#recommendationServices").empty();
                res.map(x => {
                    var html = '<tr>';
                    if (x.key <= 100) {
                        html += `
                            <td>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                            </td>
                            `
                    } else if (x.key <= 500) {
                        html += `
                            <td>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                            </td>
                            `
                    } else if (x.key <= 1000) {
                        html += `
                            <td>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                            </td>
                            `
                    } else if (x.key <= 1500) {
                        html += `
                            <td>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                            </td>
                            `
                    } else  {
                        html += `
                            <td>
                                <i class="bi bi-star-fill" style="font-size: 15px; color: cornflowerblue;"></i>
                            </td>
                            `
                    }


                    html +=`<td>${x.value}</td>
                            <td>Square difference:${x.key.toFixed(2) }</td>
                        </tr>`
                    $("#recommendationServices").append(html)
                })
            },
            error: function (ex) {
                alert('failed!');
            },
        });
}


