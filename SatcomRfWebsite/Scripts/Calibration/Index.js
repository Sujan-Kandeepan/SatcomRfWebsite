function setURL(value) {
    if (value == "default") {
        location.href = location.origin + "/Calibration/Create";
    } else {
        location.href = location.origin + "/Calibration/Create/" + value.replace(" ", "");
    }
}