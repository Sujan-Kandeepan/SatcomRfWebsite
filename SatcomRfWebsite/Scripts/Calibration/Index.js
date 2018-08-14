function calType(value) {
    if (value == "default") {
        location.href = document.getElementById("absolute-base-url").innerHTML + "Calibration";
    } else {
        location.href = document.getElementById("absolute-base-url").innerHTML + "Calibration/Index/" + value.replace(" ", "");
    }
}

function assetNum(value) {
    if (value == "default") {
        location.href = location.origin + location.pathname;
    } else {
        location.href = location.origin + location.pathname + "?assetnum=" + value.split(' ').join('_');
    }
}

function create(value) {
    if (value == "default") {
        location.href = document.getElementById("absolute-base-url").innerHTML + "Calibration/Create";
    } else {
        location.href = document.getElementById("absolute-base-url").innerHTML + "Calibration/Create/" + value.replace(" ", "");
    }
}

function getData(assetNumber) {
    $("#data-display").html("<div class='alert alert-info' role='alert'><strong>Loading...</strong></div>");
    if (assetNumber != "default") {
        $.ajax({
            type: "post",
            url: document.getElementById("absolute-base-url").innerHTML + "Calibration/GetData",
            data: { "type": $("#cal-type").val(), "assetNumber": assetNumber },
            dataType: "text",
            success: function (result) {
                var data = JSON.parse(result);

                if (data.dates.length > 0) {
                    var html = "<table class='table table-striped' id='data-table'><tr><thead><th" + ($("#cal-type").val() != "Power Sensor" ? " rowspan='2' style='vertical-align: middle'" : "") + ">Frequency</th>";
                    for (var i = 0; i < data.dates.length; i++) {
                        var details = document.URL.replace("Index", "Details") + "&date=" + data.dates[i];
                        html += "<th" + ($("#cal-type").val() != "Power Sensor" ? " colspan='2'" : "") + ">" + data.dates[i] + " (<a href='" + details + "'>Details</a>)</th>";
                    }
                    if ($("#cal-type").val() != "Power Sensor") {
                        html += "</tr><tr>";
                        for (var i = 0; i < data.dates.length; i++) {
                            html += "<th>Cal Factor</th><th>Return Loss</th>";
                        }
                    }
                    html += "</tr></thead><tbody>";
                    if (data.freqs.length == 0) {
                        html += "<tr><td colspan='" + (data.dates.length * 2 + 1) + "'>There are no records to show here!</tr>";
                    }
                    for (var i = 0; i < data.freqs.length; i++) {
                        html += "<tr><td><em>" + data.freqs[i] + "</em></td>";
                        for (var j = 0; j < data.dates.length; j++) {
                            html += "<td" + ($("#cal-type").val() == "Power Sensor" ? "" : "") + ">" + data.calFactor[j][i] + "</td>";
                            if ($("#cal-type").val() != "Power Sensor") {
                                html += "<td>" + data.returnLoss[j][i] + "</td>";
                            }
                        }
                        html += "</tr>";
                    }
                    html += "</tbody></table>";
                } else {
                    html = "<span class='center-block text-center'>There are no records to show here!</span>";
                }

                $("#data-display").html(html);
                $("#separator").show();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    } else {
        $("#data-display").html("");
        $("#separator").hide();
    }

}

$(document).ready(function () {
    document.getElementById("navBarCalibration").classList.add("active");

    $.ajax({
        type: "post",
        url: document.getElementById("absolute-base-url").innerHTML + "Calibration/GetAssetNumbers",
        data: { "type": $("#cal-type").val() },
        dataType: "text",
        success: function (result) {
            var assetNumbers = JSON.parse(result);
            var html = '<option value="default">' + ($("#cal-type").val() == "default" ? "" : "Select an asset number") + '</option>';
            for (var i = 0; i < assetNumbers.length; i++) {
                html += '<option value="' + assetNumbers[i] + '"' + (document.URL.indexOf(assetNumbers[i].split(' ').join('_')) != -1 ? ' selected' : '') + '>' + assetNumbers[i] + '</option>';
            }
            $("#asset-numbers").html(html);

            getData($("#asset-numbers").val() || "default");
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });

    if ($("#cal-type").val() == "default") {
        $("#asset-numbers").prop("disabled", true);
    } else {
        $("#asset-numbers").prop("disabled", false);
    }

    $('#data-table').dataTable({
        "scrollX": 1200
    });
});