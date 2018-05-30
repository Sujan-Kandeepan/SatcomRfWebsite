function getModelNames(productType) {
    if (productType === "default") {
        document.getElementById("models").innerHTML = "";
        document.getElementById("getdata-failed").classList.add("hide");
        document.getElementById("selection-failed").classList.add("hide");
        document.getElementById("download-failed").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetModels?productType=" + productType;
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            var html = "<option value=\"default\" selected>Choose a model.</option>";
            //html += "<option value=\"all\">All Models.</option>";

            if (retData !== null) {
                for (var i = 0; i < retData.length; i++) {
                    html += "<option value=\"" + retData[i] + "\">" + retData[i] + "</option>";
                }
            }
            else {
                html = "";
            }

            document.getElementById("models").innerHTML = html;
            document.getElementById("getdata-failed").classList.add("hide");
            document.getElementById("selection-failed").classList.add("hide");
            document.getElementById("download-failed").classList.add("hide");
            document.getElementById("data-table").innerHTML = "";
        }
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function getProductTypes() {
    var src = document.location.origin + "/api/ListAPI/GetProductTypes";
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            var html = "<option value=\"default\" selected>Choose a product type.</option>";
            for (var i = 0; i < retData.length; i++) {
                html += "<option value=\"" + retData[i] + "\">" + retData[i] + "</option>";
            }
            document.getElementById("productTypes").innerHTML = html;
        }
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function buildTable(tableData) {
    var result = "<tr><th>Test Name</th><th>Channel</th><th>Minimum</th><th>Maximum</th><th>Average</th><th>Standard Deviation</th></tr>";
    for (var i = 0; i < tableData.length; i++) {
        var unit = "";
         
        if (tableData[i].Unit === "%") {
            unit = "%";
        }
        else if (tableData[i].Unit === "X:1") {
            unit = ":1";
        }
        else {
            unit = " " + tableData[i].Unit;
        }

        var unitConv = "";
        if (unit.indexOf("dB") !== -1)
        {
            unitConv = " " + tableData[i].UnitConv;
        }

        if (unit.indexOf("dB") !== -1)
        {
            result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].MinResult + unit + "</br>"
                + tableData[i].MinResultConv + unitConv + "</td><td>" + tableData[i].MaxResult + unit + "</br>" + tableData[i].MaxResultConv + unitConv + "</td><td>"
                + tableData[i].AvgResult + unit + "</br>" + tableData[i].AvgResultConv + unitConv + "</td><td>" + tableData[i].StdDev + unit + "</br>"
                + tableData[i].StdDevConv + unitConv + "</td></tr>";
        }
        else
        {
            result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].MinResult + unit + "</td><td>"
                + tableData[i].MaxResult + unit + "</td><td>" + tableData[i].AvgResult + unit + "</td><td>" + tableData[i].StdDev + unit + "</td></tr>";
        }
    }

    return result;
}

function getTable(modelName, productType) {
    if (modelName === "default") {
        document.getElementById("data-display").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetTableData?modelName=" + modelName + "&productType=" + productType;
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        document.getElementById("data-table").innerHTML = "";
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            if (retData !== null) {
                var html = buildTable(retData);
                document.getElementById("data-table").classList.remove("hide");
                document.getElementById("data-table").innerHTML = html;
                document.getElementById("getdata-loading").classList.add("hide");
                document.getElementById("getdata-failed").classList.add("hide");
            }
            else {
                showFail();
            }
        }
        else if (this.readyState === 4 && this.status !== 200) {
            showFail();
        }
    };

    document.getElementById("data-display").classList.remove("hide");
    document.getElementById("getdata-loading").classList.remove("hide");
    document.getElementById("getdata-failed").classList.add("hide");
    document.getElementById("selection-failed").classList.add("hide");
    document.getElementById("download-failed").classList.add("hide");

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function showFail() {
    document.getElementById("getdata-failed").classList.remove("hide");
    document.getElementById("getdata-loading").classList.add("hide");
    document.getElementById("data-table").innerHTML = "";
}

function getxlsxfile(productType, modelName) {
    if (modelName === "default" || productType === "default") {
        document.getElementById("selection-failed").classList.remove("hide");
        document.getElementById("download-failed").classList.add("hide");
        return;
    }

    if (!document.getElementById("getdata-failed").classList.contains("hide")) {
        document.getElementById("download-failed").classList.remove("hide");
        document.getElementById("getdata-failed").classList.add("hide");
        return;
    }

    document.getElementById("iframe-temp").innerHTML = "<iframe style=\"display:none\" src=\"" + document.location.origin +
        "/api/ListAPI/GetTableFile?modelName=" + modelName + "&productType=" + productType + "\"></iframe>";
}

function viewResults(productType, modelName) {
    if (modelName === "default" || productType === "default") {
        document.getElementById("selection-failed").classList.remove("hide");
        document.getElementById("download-failed").classList.add("hide");
        return;
    }

    location.href = location.origin + "/testsData/TestResults/" + productType + "/" + modelName;
}

function setupIndex() {
    getProductTypes();
}