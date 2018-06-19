﻿function getModelNames(productType, selected) {
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

            sendStats();
            sendOutput(productType, selected);
            sendResults(productType, selected);
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
        sendStats();
        sendOutput(selected, "na");
        sendResults(selected, "");
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function resetFilter() {

}

function applyFilter() {

}

function buildTable(tableData) {
    var result = "<tr><th>Test Name</th><th>Channel</th><th>Power</th><th>Minimum</th><th>Maximum</th><th>Average</th><th>Standard Deviation</th><th>Cpk</th><th></th></tr>";
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
            result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].Power + "</td><td>"
                + tableData[i].MinResult + unit + "</br>" + tableData[i].MinResultConv + unitConv + "</td><td>" + tableData[i].MaxResult + unit + "</br>"
                + tableData[i].MaxResultConv + unitConv + "</td><td>" + tableData[i].AvgResult + unit + "</br>" + tableData[i].AvgResultConv + unitConv
                + "</td><td>" + tableData[i].StdDev + unit + "</br>" + tableData[i].StdDevConv + unitConv + "</td><td>" + tableData[i].Cpk + "</td><td>" +
                "<input type=\"button\" class=\"btn btn-link\" name=\"graph\" data-toggle=\"modal\" data-target=\"#allResultsModal\" onclick=\"fillModal(\'"
                + tableData[i].TestName + "\', \'" + tableData[i].Channel + "\', \'" + tableData[i].ParsableResults.toString() + "\', \'" + tableData[i].Unit
                + "\', \'" + tableData[i].UnitConv + "\', \'sn\')\" value=\"View All Results\" />" + "</td></tr>";
        }
        else
        {
            result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].Power + "</td><td>"
                + tableData[i].MinResult + unit + "</td><td>" + tableData[i].MaxResult + unit + "</td><td>" + tableData[i].AvgResult + unit + "</td><td>"
                + tableData[i].StdDev + unit + "</td><td>" + tableData[i].Cpk + "</td><td>" +
                "<input type=\"button\" class=\"btn btn-link\" name=\"graph\" data-toggle=\"modal\" data-target=\"#allResultsModal\" onclick=\"fillModal(\'"
                + tableData[i].TestName + "\', \'" + tableData[i].Channel + "\', \'" + tableData[i].ParsableResults.toString() + "\', \'" + tableData[i].Unit
                + "\', \'" + "N/A" + "\', \'sn\')\" value=\"View All Results\" />" + "</td></tr>";
        }
    }

    return result;
}

function getTable(productType, modelName, testType, tubeName, options) {
    if (modelName === "default") {
        document.getElementById("data-display").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        location.href = location.origin + "/SatcomStatsPage/Index/" + productType;
        return;
    }

    if (document.URL.indexOf(modelName) == -1) {
        location.href = location.origin + "/SatcomStatsPage/Index/" + productType + "/" + modelName;
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetTableData?modelName=" + modelName + "&productType=" + productType + "&testType=" + testType + "&tubeName=" + tubeName + "&flags=" + options;
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

        sendStats();
        sendOutput(productType, modelName);
        sendResults(productType, modelName);
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

function fillModal(testName, channel, allResultsString, unit, unitConv, sortMode) {
    var allResultsJoined = allResultsString.split(",");
    var allResults = [];

    var arrayTemp = [];
    for (var i = 0; i < allResultsJoined.length; i++) {
        if (i % 18 == 0) {
            arrayTemp = [allResultsJoined[i]];
        }
        else {
            arrayTemp.push(allResultsJoined[i]);
        }

        if (i % 18 == 17) {
            allResults.push(arrayTemp);
        }
    }

    if (sortMode == 'val') {
        for (var i = 1; i < allResults.length; i++) {
            for (var j = i; j > 0; j--) {
                if (parseFloat(allResults[j][2]) < parseFloat(allResults[j - 1][2])
                    || parseFloat(allResults[j][3]) < parseFloat(allResults[j - 1][3])) {
                    var temp = allResults[j];
                    allResults[j] = allResults[j - 1];
                    allResults[j - 1] = temp;
                }
            }
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

    if (unitConv != "N/A") {
        unitConv = " " + unitConv;
    }

    var toggleMessage = (sortMode == 'val') ? "Click to sort by serial number." : "Click to sort by value.";
    var closeButton = "<button type=\"button\" class=\"close\" style=\"float: right; margin-left: 10px\" data-dismiss=\"modal\" "
        + "aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
    var toggleButton = "<input type=\"button\" class=\"btn btn-link center-block\" name=\"graph\" onclick=\"sortMode(\'" + testName + "\', \'" + channel + "\', \'"
        + allResultsString + "\', \'" + unit + "\', \'" + unitConv + "\', \'" + sortMode + "\')\" value=\"" + toggleMessage + "\" />";
    var html = "<h4 class=\"text-center\" style=\"margin-top: 5px\">" + testName + " (Channel " + channel + ") "
        + closeButton + "</h4>" + toggleButton + "<hr/>";

    for (var i = 0; i < allResults.length; i++) {
        html += "<strong>" + allResults[i][0] + ":" + "</strong>" + " " + allResults[i][3] + unit;
        if (unitConv != "N/A") {
            html += ", " + allResults[i][4] + unitConv;
        }
        html += "</br><div style=\"margin-top: -5px; color: rgb(128, 128, 128)\">";
        html += "TestType: " + (allResults[i][1] != "" ? allResults[i][1] : "None");
        html += "</div><div style=\"margin-top: -5px; color: rgb(128, 128, 128)\">";
        html += "TubeName: " + (allResults[i][11] != "" ? allResults[i][11] : "None");
        html += "</div><div style=\"margin-top: -5px; color: rgb(128, 128, 128)\">";
        html += "Audit: " + (allResults[i][7] == "True" ? "<input type=\"checkbox\" checked disabled>" : "<input type=\"checkbox\" disabled>");
        html += "&ensp;Itar: " + (allResults[i][8] == "True" ? "<input type=\"checkbox\" checked disabled>" : "<input type=\"checkbox\" disabled>");
        html += allResults[i][12] != "" ? "&ensp;SsaSN: " + allResults[i][12] : "";
        html += allResults[i][13] != "" ? "&ensp;LinSN: " + allResults[i][13] : "";
        html += allResults[i][14] != "" ? "&ensp;LipaSN: " + allResults[i][14] : "";
        html += allResults[i][15] != "" ? "&ensp;BucSN: " + allResults[i][15] : "";
        html += allResults[i][16] != "" ? "&ensp;BipaSN: " + allResults[i][16] : "";
        html += allResults[i][17] != "" ? "&ensp;BlipaSN: " + allResults[i][17] : "";
        html += "</div><div style=\"margin-top: -5px; color: rgb(128, 128, 128)\">";
        html += "LowLimit: " + (allResults[i][4] != "" ? allResults[i][4] : "None");
        html += "&ensp;UpLimit: " + (allResults[i][5] != "" ? allResults[i][5] : "None");
        html += "</div>";
    }

    document.getElementById("content-all-results").innerHTML = html;
}

function sortMode(testName, channel, allResultsString, unit, unitConv, sortMode) {
    unit = unit.replace(" ", "");
    unitConv = unitConv.replace(" ", "");
    fillModal(testName, channel, allResultsString, unit, unitConv, (sortMode == 'val') ? 'sn' : 'val');
    $("#myModal").val(null).trigger("change");
}

function showFail() {
    document.getElementById("getdata-failed").classList.remove("hide");
    document.getElementById("getdata-loading").classList.add("hide");
    document.getElementById("data-table").innerHTML = "";
}

function getxlsxfile(productType, modelName, testType, tubeName, options) {
    if (modelName === "default" || productType === "default") {
        document.getElementById("selection-failed").classList.remove("hide");
        document.getElementById("download-failed").classList.add("hide");
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetTableFile?modelName=" + modelName + "&productType=" + productType + "&testType=" + testType + "&tubeName=" + tubeName + "&flags=" + options;
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
        "/api/ListAPI/GetTableFile?modelName=" + modelName + "&productType=" + productType + "&testType=" + testType + "&tubeName=" + tubeName + "&flags=" + options + "\"></iframe>";
}

function sendStats() {
    //document.getElementById("navBarStats").innerHTML = "<a href=\"" + location.pathname + "\">Statistics</a>";
}

function sendOutput(productType, modelName) {
    var newurl;
    if (productType == "default") {
        newurl = "/ateData/AteOutput";
    }
    else if (modelName == "default") {
        newurl = "/ateData/AteOutput/?filter=pT=" + productType + "%mN=na%ser=na%testType=na%tubeName=na%opt=na";
    }
    else {
        newurl = "/ateData/AteOutput/?filter=pT=" + productType + "%mN=" + modelName + "%ser=na%testType=na%tubeName=na%opt=na";
    }
    document.getElementById("navBarAteOutput").innerHTML = "<a href=\"" + newurl + "\">ATE Output</a>";
}

function sendResults(productType, modelName) {
    var newurl;
    if (productType == "default") {
        newurl = "/testsData/TestResults";
    }
    else if (modelName == "default") {
        newurl = "/testsData/TestResults/" + productType;
    }
    else {
        newurl = "/testsData/TestResults/" + productType + "/" + modelName;
    }
    document.getElementById("navBarTests").innerHTML = "<a href=\"" + newurl + "\">Test Results</a>";
}

function setupIndex() {
    var filter = document.URL.substring(document.URL.indexOf("Index") + 6);
    var productType = filter.indexOf("/") != -1 ? filter.substring(0, filter.indexOf("/")) : filter;
    var modelName = filter.indexOf("/") != -1 ? filter.substring(filter.indexOf("/") + 1) : "";

    if (document.URL.indexOf("SatcomStatsPage") == document.URL.length - 15) {
        getProductTypes();
    }
    else if (document.URL.indexOf("SatcomStatsPage/") == document.URL.length - 16
        || document.URL.indexOf("SatcomStatsPage/Index") == document.URL.length - 21) {
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