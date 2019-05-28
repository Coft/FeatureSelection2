var FS = {};
FS.Gui = (function () {
    var UploadTrainingSet = function () {
        var fileInput = $("#training-set-form").children("input[type='file']");

        if (fileInput.length === 0 || fileInput[0].files.length === 0) {
            return;
        }

        var file = fileInput[0].files[0];
        var formData = new FormData();
        formData.append("file", file);

        $.ajax({
            url: CAPA.UrlHelper.BaseUrl("Record/AddAttachment"),
            type: "post",
            data: formData,
            processData: false,
            contentType: false
        })
            .done($.proxy(function (data) {
            }));
    };

    var UploadTestSet = function () {
        var fileInput = currentElement.siblings("input[type='file']");

        if (fileInput.length == 0 || fileInput[0].files.length == 0) {
            return;
        }

        var file = fileInput[0].files[0];
    };

    var GeneralButtons = {
        UploadTraining: $("#training-set-form button").click(UploadTrainingSet),
        UploadTest: $("#test-set-form button").click(UploadTestSet),
    };

    var GeneralInputs = {
    };

    var Container = {
    };

    var Templates = {
    };

    return {
        GeneralButtons: GeneralButtons,
        GeneralInputs: GeneralInputs,
        Container: Container,
        Templates: Templates,
    };
})();