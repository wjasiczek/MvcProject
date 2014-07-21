$(function () {
    var form = $("form");
    var formRemainder = $("form #crcModel_remainder");
    var formSignal = $("form #crcModel_signal");

    var target = $("#submitResult");
    target.delay(3000).fadeTo(1000, 0);

    if (target.html() === "Data already in database") {
        formRemainder.val("");
        formSignal.val("");
    }

    function PrintRemainder() {
        var remainderLabel = $("#remainderLabel");
        var labelText = "<b>Remainder: </b>";

        UpdateResults(formRemainder, remainderLabel, labelText);
    }

    function PrintSignal() {
        var signalLabel = $("#signalLabel");
        var labelText = "<b>Signal: </b>";

        UpdateResults(formSignal, signalLabel, labelText);
    }

    function InputChange() {
        formRemainder.val("");
        formSignal.val("");
        $("#signalLabel").html("");
        $("#remainderLabel").html("");
        $("#correctnessLabel").html("");
    }

    function CheckCorrectness() {
        if (!form.valid() || formSignal.val() === "")
        {
            console.log("hehe");
            var target = $("#submitResult");
            target.html("<b>Submit valid values first!</b>");
            target.stop(true, true);
            target.fadeTo(1000, 1);
            target.delay(3000).fadeTo(1000, 0);
            return;
        }

        var options = {
            url: "GetCorrectness",
            type: form.attr("method"),
            data: {
                signal: formSignal.val(),
                remainder: formRemainder.val()
            }
        };

        $.ajax(options).done(function (data) {
            var correctnessLabel = $("#correctnessLabel");

            UpdateResults(formRemainder, correctnessLabel, "", data.message);
        });
    }

    function UpdateResults(input, label, labelText, message) {
        var target = $("#submitResult");

        if (form.valid() && input.val() !== "") {
            label.html(labelText);
            var value = message !== undefined ? message : input.val();
            label.append(value);
        } else {
            label.html("");
            target.html("<b>Submit valid values first!</b>");
            target.stop(true, true);
            target.fadeTo(1000, 1);
            target.delay(3000).fadeTo(1000, 0);
        }       
    }

    function CheckListItem() {
        $.ajax({
            url: this.href,
            type: 'GET'
        }).done(function (data) {
            $("#" + data.id).replaceWith("<span>" + data.message + "</span>");
        });

        return false;
    }

    function AddFilter() {
        var activeId = $(this).attr("id");

        if (activeId !== previousRadioButton)
        {
            previousRadioButton = activeId;

            $.ajax({
                url: "CrcList",
                type: "GET",
                data: { filter: activeId }
            }).done(function (data) {
                $("#testList").html(data);
            });
        }
    }

    var GetPage = function () {
        var $a = $(this);
        var activeId = $("label.active").attr("id");

        var options = {
            url: $a.attr("href") + "&filter=" + activeId,
            type: "GET",
        };

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-mvcproj-target");
            $(target).replaceWith(data);
        });
        return false;
    };

    function DownloadFile() {
        var selectedFile = $(".files option:selected").val();
        if (selectedFile) {
            $(this).attr("href", "/Download/Download?fileName=" + selectedFile);
        } else {
            return false;
        }
    }


    var previousRadioButton = "All";

    $("#buttonRemainder").click(PrintRemainder);
    $("#buttonSignal").click(PrintSignal);
    $("#buttonCorrectness").click(CheckCorrectness);
    $("form #crcModel_binaryValue").on("input", InputChange);
    $("form #crcModel_generator").on("input", InputChange);
    $(".Checks a").click(CheckListItem);
    $("#radioButtons label").click(AddFilter);
    $(".body-content").on("click", ".pagedList a", GetPage);
    $("#downloadLink").click(DownloadFile);
});