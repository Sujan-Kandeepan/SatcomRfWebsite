function setURL(value) {
    if (value == "default") {
        location.href = location.origin + "/Calibration";
    } else {
        location.href = location.origin + "/Calibration/Index/" + value.replace(" ", "");
    }
}