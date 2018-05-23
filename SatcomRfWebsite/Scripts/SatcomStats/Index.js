function getModelNames(familyName) {
    if (familyName === "default") {
        document.getElementById("models").innerHTML = "";
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetModels?familyName=" + familyName;
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
        }
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function getFamilies() {
    var src = document.location.origin + "/api/ListAPI/GetFamilies";
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            var html = "<option value=\"default\" selected>Choose a model family.</option>";
            for (var i = 0; i < retData.length; i++) {
                html += "<option value=\"" + retData[i] + "\">" + retData[i] + "</option>";
            }
            document.getElementById("families").innerHTML = html;
        }
    };

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function showFail() {
    var alertHTML = "<div class=\"alert alert-danger alert-dismissible fade in\" id=\"alert-failed\"><a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a><strong>Error:</strong> Failed to get data from server.</div>";
    document.getElementById("getdata-failed").innerHTML = alertHTML;
    document.getElementById("getdata-failed").classList.remove("hide");
}

function buildTable(tableData) {
    var result = "<tr><th>Test Name</th><th>Channel</th><th>Min</th><th>Max</th><th>Average</th><th>Standard Deviation</th></tr>";
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

        result += "<tr><td>" + tableData[i].TestName + "</td><td>" + tableData[i].Channel + "</td><td>" + tableData[i].MinResult +
            unit + "</td><td>" + tableData[i].MaxResult + unit + "</td><td>" + tableData[i].AvgResult + unit + "</td><td>" + tableData[i].StdDev + unit + "</td></tr>";
    }

    return result;
}

function getTable(modelName, familyName) {
    if (modelName === "default") {
        document.getElementById("data-display").classList.add("hide");
        document.getElementById("data-table").innerHTML = "";
        return;
    }

    var src = document.location.origin + "/api/ListAPI/GetTableData?modelName=" + modelName + "&familyName=" + familyName;
    var data = new XMLHttpRequest();
    data.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            var retData = JSON.parse(data.responseText);
            if (retData !== null) {
                var html = buildTable(retData);
                document.getElementById("data-table").classList.remove("hide");
                document.getElementById("data-table").innerHTML = html;
                document.getElementById("getdata-loading").classList.add("hide");
            }
            else {
                showFail();
            }
        }
        else if (this.readyState === 4 && this.status !== 200) {
            document.getElementById("getdata-loading").classList.add("hide");
            showFail();
        }
    };

    document.getElementById("data-display").classList.remove("hide");
    document.getElementById("getdata-loading").classList.remove("hide");

    data.open("GET", src, true);
    data.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    data.send();
}

function showSelectionError() {
    var alertHTML = "<div class=\"alert alert-danger alert-dismissible fade in\" id=\"alert-failed\"><a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a><strong>Error:</strong> Select a family or model before downloading report.</div>";
    document.getElementById("selection-failed").innerHTML = alertHTML;
    document.getElementById("selection-failed").classList.remove("hide");
}

function getxlsxfile(modelName, familyName) {
    if (modelName === "default" || familyName === "default") {
        showSelectionError();
        return;
    }

    document.getElementById("iframe-temp").innerHTML = "<iframe style=\"display:none\" src=\"" + document.location.origin +
        "/api/ListAPI/GetTableFile?modelName=" + modelName + "&familyName=" + familyName + "\"></iframe>";
}

function setupIndex() {
    getFamilies();
}