function getModelNames(productType, selected) {
    selected = selected || "default";
    if (productType === "default") {
        document.getElementById("models").innerHTML = "";
        document.getElementById("getdata-failed").classList.add("hide");
        document.getElementById("selection-failed").classList.add("hide");
        document.getElementById("download-failed").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        location.href = location.origin + "/SatcomStatsPage";
        return;
    }

    if (document.URL.indexOf(productType) == -1) {
        location.href = location.origin + "/SatcomStatsPage/Index/" + productType;
    }

    var src = document.location.origin + "/api/ListAPI/GetModels?productType=" + productType;
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            var html = "<option value=\"default\"" + (selected == "default" ? " selected" : "") + ">Choose a model.</option>";
            //html += "<option value=\"all\">All Models.</option>";

            if (retData !== null) {
                for (var i = 0; i < retData.length; i++) {
                    html += "<option value=\"" + retData[i] + "\"" + (selected == retData[i] ? " selected" : "") + ">" + retData[i] + "</option>";
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

function getProductTypes(selected) {
    selected = selected || "default";
    var src = document.location.origin + "/api/ListAPI/GetProductTypes";
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var found = selected == "default";
            var retData = JSON.parse(data.responseText);
            var html = "<option value=\"default\"" + (selected == "default" ? " selected" : "") + ">Choose a product type.</option>";
            for (var i = 0; i < retData.length; i++) {
                html += "<option value=\"" + retData[i] + "\"" + (selected == retData[i] ? " selected" : "") + ">" + retData[i] + "</option>";
                if (selected == retData[i]) {
                    found = true;
                }
            }
            document.getElementById("productTypes").innerHTML = html;
            if (!found) {
                document.getElementById("invalid-url").classList.remove("hide");
                setTimeout(function () { location.replace(location.origin + "/SatcomStatsPage"); }, 1000);
            }
        }
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function buildTable(tableData) {
    var result = "<tr><th>Test Name</th><th>Channel</th><th>Minimum</th><th>Maximum</th><th>Average</th><th>Standard Deviation</th><th></th></tr>";
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
                + tableData[i].MinResultConv + unitConv + "</td><td>" + tableData[i].MaxResult + unit + "</br>" + tableData[i].MaxResultConv
                + unitConv + "</td><td>" + tableData[i].AvgResult + unit + "</br>" + tableData[i].AvgResultConv + unitConv + "</td><td>"
                + tableData[i].StdDev + unit + "</br>" + tableData[i].StdDevConv + unitConv + "</td><td>" +
                "<input type=\"button\" class=\"btn btn-link\" name=\"graph\" data-toggle=\"modal\" data-target=\"#allResultsModal\" onclick=\"fillModal(\'" + tableData[i].TestName + "\', \'" + tableData[i].Channel + "\', \'" + tableData[i].AllResults.toString() +
                "\', \'" + tableData[i].AllResultsConv.toString() + "\', \'" + tableData[i].Unit + "\', \'" + tableData[i].UnitConv +
                "\')\" value=\"View All Results\" />" + "</td></tr>";
        }
        else
        {
            result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].MinResult + unit + "</td><td>"
                + tableData[i].MaxResult + unit + "</td><td>" + tableData[i].AvgResult + unit + "</td><td>" + tableData[i].StdDev + unit + "</td><td>" +
                "<input type=\"button\" class=\"btn btn-link\" name=\"graph\" data-toggle=\"modal\" data-target=\"#allResultsModal\" onclick=\"fillModal(\'" + tableData[i].TestName + "\', \'" + tableData[i].Channel + "\', \'" + tableData[i].AllResults.toString() +
                "\', \'N/A\', \'" + tableData[i].Unit + "\', \'N/A\')\" value=\"View All Results\" />" + "</td></tr>";
        }
    }

    return result;
}

function getTable(productType, modelName) {
    if (modelName === "default") {
        document.getElementById("data-display").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        return;
    }

    if (document.URL.indexOf(modelName) == -1) {
        location.href = location.origin + "/SatcomStatsPage/Index/" + productType + "/" + modelName;
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

function fillModal(testName, channel, allResultsString, allResultsConvString, unit, unitConv) {
    var allResultsJoined = allResultsString.split(",");
    var allResultsConvJoined = allResultsConvString.split(",");
    var allResultsSerials = [], allResultsValues = [], allResultsConvValues = [];

    for (var i = 0; i < allResultsJoined.length; i++) {
        if (i % 2 == 0) {
            allResultsSerials.push(allResultsJoined[i]);
        } else {
            allResultsValues.push(allResultsJoined[i]);
            allResultsConvValues.push(allResultsConvJoined[i]);
        }
    }

    if (unit === "%") {
        unit = "%";
    }
    else if (unit === "X:1") {
        unit = ":1";
    }
    else {
        unit = " " + unit;
    }
    unitConv = " " + unitConv;

    var closeButton = "<button type=\"button\" class=\"close\" style=\"float: right; margin-left: 10px\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
    var html = "<h4 class=\"text-center\" style=\"margin-top: 5px\">" + testName + " (Channel " + channel + ") " + closeButton + "</h4>" + "<hr/>";
    for (var i = 0; i < allResultsSerials.length; i++) {
        html += "<strong>" + allResultsSerials[i] + ":" + "</strong>" + " " + allResultsValues[i] + unit;
        if (unitConv != " N/A") {
            html += ", " + allResultsConvValues[i] + unitConv;
        }
        html +="</br>";
    }

    document.getElementById("content-all-results").innerHTML = html;
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

    var src = document.location.origin + "/api/ListAPI/GetTableFile?modelName=" + modelName + "&productType=" + productType;
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            document.getElementById("excel-loading").classList.add("hide");
        } else if (this.readyState == 4 && this.status !== 200) {
            document.getElementById("excel-loading").classList.add("hide");
            document.getElementById("excel-failed").classList.remove("hide");
        }
    };

    document.getElementById("excel-loading").classList.remove("hide");

    data.open("GET", src, true);
    data.send();

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
    var filter = document.URL.substring(document.URL.indexOf("Index") + 6);
    var productType = filter.indexOf("/") != -1 ? filter.substring(0, filter.indexOf("/")) : filter;
    var modelName = filter.indexOf("/") != -1 ? filter.substring(filter.indexOf("/") + 1) : "";

    if (document.URL.indexOf("SatcomStatsPage") == document.URL.length - 15) {
        getProductTypes();
    }
    else if (document.URL.indexOf("SatcomStatsPage/Index") == document.URL.length - 21) {
        location.replace(location.origin + "/SatcomStatsPage");
    }
    else {
        getProductTypes(productType);
        if (modelName != "") {
            getModelNames(productType, modelName);
            getTable(productType, modelName);
        }
        else {
            getModelNames(productType);
        }
    }
}