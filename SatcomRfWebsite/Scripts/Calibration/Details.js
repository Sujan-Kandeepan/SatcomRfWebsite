var type = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
var assetnum = document.URL.substring(document.URL.indexOf("assetnum=") + 9, document.URL.indexOf("date=") - 1).split('_').join(' ');
var date = document.URL.substring(document.URL.indexOf("date=") + 5);

$.ajax({
    type: "post",
    url: "/Calibration/GetDetails",
    data: { "type": type, "assetnum": assetnum, "date": date },
    datatype: "json",
    success: function (result) {
        if (result == "Fail") return;
        var headers = JSON.parse(result.headers);
        if (type == "Attenuator" || type == "OutputCoupler") {
            $("#StartFreq").html(headers.StartFreq);
            $("#StopFreq").html(headers.StopFreq);
            $("#Points").html(headers.Points);
            $("#Loss").html(headers.Loss);
            $("#Power").html(headers.Power);
            $("#MaxOffset").html(headers.MaxOffset);
            $("#Temp").html(headers.Temp);
            $("#Humidity").html(headers.Humidity);
            $("#Lookback").html(headers.Lookback);
            $("#Operator").html(headers.Operator);
            $("#ExpireDate").html(headers.ExpireDate);
            $("#AddedDate").html(headers.AddedDate);
            $("#EditedBy").html(headers.EditedBy);
        } else if (type == "PowerSensor") {
            $("#Series").html(headers.Series);
            $("#Serial").html(headers.Serial);
            $("#RefCal").html(headers.RefCal);
            $("#Certificate").html(headers.Certificate);
            $("#Operator").html(headers.Operator);
            $("#AddedDate").html(headers.AddedDate);
            $("#EditedBy").html(headers.EditedBy);
        }

        $("#records-display").show();
        if (result.freqs.length == 0) {
            $("#records-display").html("<div class='container' style='margin-top: 20px'><div class='alert alert-warning'><strong>This item has no calibration data.</strong></div></div>");
            return;
        }

        var freqs = "", calFactor = "", returnLoss = "";
        for (var i = 0; i < result.freqs.length; i++) {
            freqs += "<td>" + result.freqs[i] + "</td>";
            calFactor += "<td>" + result.calFactor[i] + "</td>";
            if (document.URL.indexOf("PowerSensor") == -1) {
                returnLoss += "<td>" + result.returnLoss[i] + "</td>";
            }
        }

        $("#freqs").html(freqs);
        $("#calFactor").html(calFactor);
        if (document.URL.indexOf("PowerSensor") == -1) {
            $("#returnLoss").html(returnLoss);
        }

    },
    error: function (xhr, status, error) {
        alert("Something went wrong!");
    }
});

$(document).ready(function () {
    document.getElementById("navBarCalibration").classList.add("active");

    $("#header").html(assetnum + " (" + date + ")");

    $("#index").click(function () {
        location.href = document.URL.substring(0, document.URL.indexOf("&")).replace("Details", "Index");
    });

    $("#edit").click(function () {
        location.href = document.URL.replace("Details", "Edit");
    });

    $("#delete").click(function () {
        $("#default-buttons").hide();
        $("#delete-buttons").show();
    });

    $("#cancel-delete").click(function () {
        $("#delete-buttons").hide();
        $("#default-buttons").show();
    });

    $("#confirm-delete").click(function () {
        $.ajax({
            type: "post",
            url: "/Calibration/Delete",
            data: { "type": type, "assetnum": assetnum, "date": date },
            datatype: "json",
            success: function (result) {
                console.log(result);
                if (result == "Fail") {
                    alert("Something went wrong while deleting the record!")
                    $("#cancel-delete").trigger("click");
                } else {
                    $("#index").trigger("click");
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                $("#cancel-delete").trigger("click");
            }
        });
    });

    $('#records').dataTable({
        "scrollX": 1200,
        "bLengthChange": false,
        "bFilter": false,
        "bPaginate": false,
        "bInfo": false
    });

    $("table").show();

    $.ajax({
        type: "post",
        url: "/Calibration/GenerateTextFile",
        data: { "type": type, "assetnum": assetnum, "date": date },
        datatype: "json",
        success: function (result) {
            $("#txt-display").html(result);
            $("#txt-display").height(0);
            $("#txt-display").height($("#txt-display").prop("scrollHeight"));

            $("#toggle-txt-expand").click(function () {
                if ($(this).html() == "Collapse") {
                    $(this).html("Expand");
                    $("#txt-display").html(result.split("\r\n")[0] + "\r\n...");
                    $("#txt-display").height(0);
                    $("#txt-display").height($("#txt-display").prop("scrollHeight"));
                } else {
                    $(this).html("Collapse");
                    $("#txt-display").html(result);
                    $("#txt-display").height(0);
                    $("#txt-display").height($("#txt-display").prop("scrollHeight"));
                }
            });
            $("#toggle-txt-expand").click();

            $("#download-txt").click(function () {
                var text = result;
                var filename = assetnum.replace("/", "_") + ".txt";
                var blob = new Blob([text], { type: "text/plain;charset=utf-8" });
                saveAs(blob, filename);

                try {
                    var isFileSaverSupported = !!new Blob;
                    if (!isFileSaverSupported) {
                        $("#download-txt").attr('data-content', "Could not save file!");
                    } else {
                        document.execCommand("copy");
                        $("#download-txt").attr('data-content', "Saved to Downloads!");
                    }
                    $("#download-txt").popover("show");
                    setTimeout(function () {
                        $("#download-txt").popover("destroy");
                    }, 3000);
                } catch (e) { }
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
            $("#cancel-delete").trigger("click");
        }
    });

    var folder = "";
    if (type == "Attenuator") folder = "Attenuator";
    if (type == "OutputCoupler") folder = "Output Coupler";
    if (type == "PowerSensor") folder = "Power Sensor";
    $("#pathstring").val("P:\\CalFiles\\" + folder + "\\ATE_TEXT\\" + assetnum.replace("/", "_") + ".txt");

    new ClipboardJS('.btn');
    $("#copy-txt").click(function () {
        if ($("#toggle-txt-expand").html() != "Collapse") $("#toggle-txt-expand").click();
        $("#txt-display").select();
        if (!ClipboardJS.isSupported()) {
            $("#copy-txt").attr('data-content', "Press Ctrl-C");
        } else {
            document.execCommand("copy");
            $("#copy-txt").attr('data-content', "Copied!");
        }
        $("#copy-txt").popover("show");
        setTimeout(function () {
            $("#copy-txt").popover("destroy");
        }, 3000);
    });
});