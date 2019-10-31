"use strict";

$(document).ready(function() {
    $("#privacy-policy-check").prop("checked", false);
});

$("#privacy-policy-check").on("change", function () {
    if ($("#privacy-policy-check").prop("checked"))
        $("#registration-btn").prop("disabled", false);
    else
        $("#registration-btn").prop("disabled", true);
})