function setURL(value) {
    if (value == "default") {
        location.href = location.origin + "/Calibration/Create";
    } else {
        location.href = location.origin + "/Calibration/Create/" + value.replace(" ", "");
    }
}

function index(value) {
    if (value == "default") {
        location.href = location.origin + "/Calibration";
    } else {
        location.href = location.origin + "/Calibration/Index/" + value.replace(" ", "");
    }
}

$(document).ready(function () {
    $('form').not("#AssetNumber").disableAutoFill();

    $("input").not(".btn").attr("data-lpignore", "true");

    document.getElementById("navBarCalibration").classList.add("active");

    if (document.URL.indexOf("failed") != -1) {
        alert("Could not submit to the database!");
        location.replace(document.URL.substring(0, document.URL.indexOf("?")));
    }

    $(".datepicker").datepicker({
        format: 'mm/dd/yyyy',
        changeMonth: true,
        changeYear: true
    });

    $("#reset-form").click(function () {
        $("form input").not(".btn").val("");
        $("#DataFields").html('<span style="color: grey">Form fields to enter calfactor records will appear here</span>');
        $("#validation-message").hide();
        $("#validation-message").html("");
    });

    $("input:text,form").click(function () {
        $(this).attr("autocomplete", "off");
    });

    $("input:file").on("change", function () {
        $("#import-msg").html('Getting form data...');
        $("#file-browser").prop("disabled", true);
        $("form input").prop("disabled", true);

        if ($(this).prop("files").length > 0 && window.FormData !== undefined) {
            var file = $(this).prop("files")["files" in $(this) ? $(this).prop("files").length - 1 : 0];
            var data = new FormData();
            data.append('file', file);

            $.ajax({
                type: "post",
                url: "/Calibration/ImportFile?type=" + $("#device-type").val().replace(" ", ""),
                data: data,
                contentType: false,
                processData: false,
                success: function (result) {
                    if (result == "Fail") {
                        $("#import-msg").html('Import calibration file');
                        $("#file-browser").prop("disabled", false);
                        $("form input").prop("disabled", false);

                        alert("Could not parse file!");
                        return;
                    }

                    var fields = JSON.parse(result);
                    if ($("#device-type").val() == "Attenuator" || $("#device-type").val() == "Output Coupler") {
                        var calDateReturned = fields.CalDate.substring(0, fields.CalDate.indexOf("T"));
                        var calDatePieces = calDateReturned.split('-');
                        var calDate = calDatePieces[1] + "/" + calDatePieces[2] + "/" + calDatePieces[0];

                        var expireDateReturned = fields.ExpireDate.substring(0, fields.ExpireDate.indexOf("T"));
                        var expireDatePieces = expireDateReturned.split('-');
                        var expireDate = expireDatePieces[1] + "/" + expireDatePieces[2] + "/" + expireDatePieces[0];

                        $("#AssetNumber").val(fields.AssetNumber);
                        $("#Operator").val(fields.Operator);
                        $("#CalDate").val(calDate);
                        $("#StartFreq").val(fields.StartFreq);
                        $("#StopFreq").val(fields.StopFreq);
                        $("#ExpireDate").val(expireDate);
                        $("#Loss").val(fields.Loss);
                        $("#Power").val(fields.Power);
                        $("#MaxOffset").val(fields.MaxOffset);
                        $("#Temp").val(fields.Temp);
                        $("#Humidity").val(fields.Humidity);
                        $("#Lookback").val(fields.Lookback);
                        $("#Points").val(fields.Records.length);
                    } else if ($("#device-type").val() == "Power Sensor") {
                        var dateReturned = fields.CalDate.substring(0, fields.CalDate.indexOf("T"));
                        var pieces = dateReturned.split('-');
                        var date = pieces[1] + "/" + pieces[2] + "/" + pieces[0];

                        $("#AssetNumber").val(fields.AssetNumber);
                        $("#Operator").val(fields.Operator);
                        $("#CalDate").val(date);
                        $("#Series").val(fields.Series);
                        $("#Serial").val(fields.Serial);
                        $("#RefCal").val(fields.RefCal);
                        $("#Certificate").val(fields.Certificate);
                        $("#Points").val(fields.Records.length);
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
                            $("#import-msg").html('Import calibration file');
                            $("#file-browser").prop("disabled", false);
                            $("form input").prop("disabled", false);

                            setTimeout(function () {
                                for (var i = 0; i < fields.Records.length; i++) {
                                    var record = fields.Records[i];
                                    $("#Records_" + i.toString() + "__Frequency").val(record.Frequency.toString());
                                    $("#Records_" + i.toString() + "__CalFactor").val(record.CalFactor.toString());
                                }
                            }, 0);
                        },
                        error: function (xhr, status, error) {
                            $("#DataFields").html(xhr.responseText);
                            $("#import-msg").html('Import calibration file');
                            $("#file-browser").prop("disabled", false);
                            $("form input").prop("disabled", false);
                        }
                    });
                },
                error: function (xhr, status, p3, p4) {
                    $("#import-msg").html('Import calibration file');
                    $("#file-browser").prop("disabled", false);
                    $("form input").prop("disabled", false);

                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            alert("This browser does not support file uploads!");
        }
        $(this).val("");
    });

    function fillForm() {
        var type = $("#device-type").val().replace(" ", "");
        $.ajax({
            type: "post",
            url: "/Calibration/GetDetails",
            data: { "type": type, "assetnum": $("#AssetNumber").val() },
            datatype: "json",
            success: function (result) {
                if (result == "Fail") {
                    var assetnum = $("#AssetNumber").val();
                    $("#reset-form").click();
                    $("#AssetNumber").val(assetnum);
                    return;
                }

                var headers = JSON.parse(result.headers);
                if (type == "Attenuator" || type == "OutputCoupler") {
                    $("#Operator").val(headers.Operator);
                    $("#StartFreq").val(headers.StartFreq);
                    $("#StopFreq").val(headers.StopFreq);
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

                    $("#Operator").val(headers.Operator);
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

    $.ajax({
        type: "post",
        url: "/Calibration/GetAssetNumbers",
        data: {
            "type": $("#device-type").val()
        },
        dataType: "text",
        success: function (result) {
            $("#AssetNumber").autocomplete({
                source: JSON.parse(result),
                minLength: 0,
                close: function (event, ui) {
                    $("#assetnum-not-recognized").hide();
                    fillForm();
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
            console.log(error);
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
                "type": $("#device-type").val().replace(" ", ""),
                "formString": serializeForm()
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
                    $("#validation-message").html("<strong>Form could not be submitted!</strong> "
                        + "The following validation errors were present:</br>"
                        + result.message);
                    $("#validation-message").show();
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