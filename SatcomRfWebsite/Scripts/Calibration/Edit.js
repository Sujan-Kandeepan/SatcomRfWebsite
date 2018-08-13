var type, assetnum, date;

function fillForm() {
    $.ajax({
        type: "post",
        url: "/Calibration/GetDetails",
        data: { "type": type, "assetnum": assetnum, "date": date },
        datatype: "json",
        success: function (result) {
            if (result == "Fail") {
                alert("Could not retrieve existing data!");
                return;
            }

            var headers = JSON.parse(result.headers);
            if (type == "Attenuator" || type == "OutputCoupler") {
                $("#AssetNumber").val(headers.AssetNumber);
                $("#Operator").val(headers.Operator);
                $("#CalDate").val(headers.CalDate);
                $("#StartFreq").val(headers.StartFreq);
                $("#StopFreq").val(headers.StopFreq);
                $("#ExpireDate").val(headers.ExpireDate);
                $("#Loss").val(headers.Loss);
                $("#Power").val(headers.Power);
                $("#MaxOffset").val(headers.MaxOffset);
                $("#Temp").val(headers.Temp);
                $("#Humidity").val(headers.Humidity);
                $("#Lookback").val(headers.Lookback);
                $("#Points").val(result.freqs.length);
            } else if (type == "PowerSensor") {
                var dateReturned = headers.CalDate.substring(0, headers.CalDate.indexOf("T"));
                var pieces = dateReturned.split('-');
                var date = pieces[1] + "/" + pieces[2] + "/" + pieces[0];

                $("#AssetNumber").val(headers.AssetNumber);
                $("#Operator").val(headers.Operator);
                $("#CalDate").val(headers.CalDate);
                $("#Series").val(headers.Series);
                $("#Serial").val(headers.Serial);
                $("#RefCal").val(headers.RefCal);
                $("#Certificate").val(headers.Certificate);
                $("#Points").val(result.freqs.length);
            }

            var freqs = [], calFactor = [], returnLoss = [];
            for (var i = 0; i < $("#DataFields > .row").length; i++) {
                freqs.push($("#Records_" + i.toString() + "__Frequency").val());
                calFactor.push($("#Records_" + i.toString() + "__CalFactor").val());
                returnLoss.push($("#Records_" + i.toString() + "__ReturnLoss").val());
            }

            $.ajax({
                type: "post",
                url: "/Calibration/CreateDataFields",
                data: {
                    "num": $("#Points").val() > 0 ? $("#Points").val() : 0,
                    "hasReturnLoss": document.URL.indexOf("PowerSensor") == -1 ? true : false,
                    "freqs": JSON.stringify(freqs),
                    "calFactor": JSON.stringify(calFactor),
                    "returnLoss": JSON.stringify(returnLoss)
                },
                dataType: "text",
                success: function (result2) {
                    $("#DataFields").html(result2);
                    $("form input").prop("disabled", false);

                    setTimeout(function () {
                        for (var i = 0; i < result.freqs.length; i++) {
                            $("#Records_" + i.toString() + "__Frequency").val(result.freqs[i].toString());
                            $("#Records_" + i.toString() + "__CalFactor").val(result.calFactor[i].toString());
                            $("#Records_" + i.toString() + "__ReturnLoss").val(result.returnLoss[i].toString());
                        }
                    }, 0);
                },
                error: function (xhr, status, error) {
                    $("#DataFields").html(xhr.responseText);
                    $("form input").prop("disabled", false);
                }
            });
        },
        error: function (xhr, status, p3, p4) {
            $("form input").prop("disabled", false);

            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
        }
    });
}

$(document).ready(function () {
    $('form').disableAutoFill();
    $("form input").prop("disabled", true);

    $("input").not(".btn").attr("data-lpignore", "true");

    document.getElementById("navBarCalibration").classList.add("active");

    type = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
    assetnum = document.URL.substring(document.URL.indexOf("assetnum=") + 9, document.URL.indexOf("date=") - 1).split('_').join(' ');
    date = document.URL.substring(document.URL.indexOf("date=") + 5);

    $("#TypePassed").val(type);
    $("#AssetNumberPassed").val(assetnum);
    $("#DatePassed").val(date);

    $(".datepicker").datepicker({
        format: 'mm/dd/yyyy',
        changeMonth: true,
        changeYear: true
    });

    $("#title").html("Edit " + assetnum + " (" + date + ")");

    fillForm();

    $("#reset").click(function () {
        $("#DataFields").html('<span style="color: grey">Form fields to enter calfactor records will appear here</span>');
        fillForm();
    });

    $("#details").click(function () {
        location.href = document.URL.replace("Edit", "Details");
    });

    $("input:text,form").click(function () {
        $(this).attr("autocomplete", "off");
    });

    $.ajax({
        type: "post",
        url: "/Calibration/GetAssetNumbers",
        data: {
            "type": type
        },
        dataType: "text",
        success: function (result) {
            $("#AssetNumber").autocomplete({
                source: JSON.parse(result),
                minLength: 0,
                close: function (event, ui) {
                    $("#assetnum-not-recognized").hide();
                }
            }).focusout(function () {
                if (JSON.parse(result).indexOf($("#AssetNumber").val()) == -1 && $("#AssetNumber").val() != "") {
                    setTimeout(function () { $("#assetnum-not-recognized").show(); }, 0);
                } else {
                    $("#assetnum-not-recognized").hide();
                }
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });

    $("#Points").on("input change", function () {
        if ($("#Points").val().length > 3) {
            $("#Points").val($("#Points").val().substring(0, 3));
        }

        var freqs = [], calFactor = [], returnLoss = [];
        for (var i = 0; i < $("#DataFields > .row").length; i++) {
            freqs.push($("#Records_" + i.toString() + "__Frequency").val());
            calFactor.push($("#Records_" + i.toString() + "__CalFactor").val());
            returnLoss.push($("#Records_" + i.toString() + "__ReturnLoss").val());
        }

        $.ajax({
            type: "post",
            url: "/Calibration/CreateDataFields",
            data: {
                "num": $("#Points").val() > 0 ? $("#Points").val() : 0,
                "hasReturnLoss": document.URL.indexOf("PowerSensor") == -1 ? true : false,
                "freqs": JSON.stringify(freqs),
                "calFactor": JSON.stringify(calFactor),
                "returnLoss": JSON.stringify(returnLoss)
            },
            dataType: "text",
            success: function (result) {
                $("#DataFields").html(result);
            },
            error: function (xhr, status, error) {
                $("#DataFields").html(xhr.responseText);
            }
        });
    });

    function serializeForm() {
        var obj = {};
        var elements = $("form").find("input").not(".btn");
        for (var i = 0; i < elements.length; ++i) {
            var element = elements[i];
            var name = element.name;
            var value = element.value;

            if (name) {
                obj[name] = value;
            }
        }

        return JSON.stringify(obj);
    }

    $("form").submit(function (e) {
        e.preventDefault();
        $("#assetnum-not-recognized").hide();
        $.ajax({
            type: "post",
            url: "/Calibration/ValidateForm",
            data: {
                "type": type,
                "formString": serializeForm(),
                "mode": "edit"
            },
            dataType: "json",
            success: function (result) {
                if (result.isValid) {
                    $("form").unbind("submit");
                    $("#validation-message").hide();
                    $("#validation-message").html("");
                    $("#success").show();
                    $(".btn-success").click();
                    return true;
                }
                else {
                    $("#validation-message").show();
                    $("#validation-message").html("<strong>Form could not be submitted!</strong> "
                        + "The following validation errors were present:</br>"
                        + result.message);
                    return false;
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                return false;
            }
        });
    });
});