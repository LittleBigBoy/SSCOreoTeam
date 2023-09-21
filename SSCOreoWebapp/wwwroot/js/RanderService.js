initPage()
function initPage() {
    initClient();
    renderChart();
    initServices();
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
                console.log(res);
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
                    var html = `<div class="item row g-2">
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
                document.addEventListener("DOMContentLoaded", function () {
                    const leftPanel = document.querySelector(".left-panel");
                    const rightPanel = document.querySelector(".right-panel");

                    const items = document.querySelectorAll(".item");

                    items.forEach((item) => {
                        item.addEventListener("dblclick", () => {
                            if (item.parentElement === leftPanel) {
                                rightPanel.appendChild(item);
                            } else {
                                leftPanel.appendChild(item);
                            }
                        });
                    });
                });
            },
            error: function (ex) {
                alert('failed! exception: ' + ex);
            },
        });
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
                var datas = res.map(x => x.amount)
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


