﻿var type = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
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
});