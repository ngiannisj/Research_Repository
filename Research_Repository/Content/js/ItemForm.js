$(document).ready(function () {
    $("#file-upload-box").change(function () {
        handleFiles(this);
    });

    $("#team-selector").change(function () {
        if ($(this).val() != 0) {
            $("#project-selector").prop('disabled', false);
        }
    });

    $("#theme-selector").change(function () {
        if ($(this).val() != 0) {
            $("#tag-selector select").prop('disabled', false);
        }
    });

    $("#status-selector").change(function () {
        if ($(this).val() == "Rejected") {
            $("#comment-input").show();
        } else {
            $("#comment-input").hide();
        }
    });

});

function addKeyInsightField(buttonRef) {
    const numberOfKeyInsights = $(buttonRef).parent().find(".key-insight-field").length;
    const lastKeyInsight = $(buttonRef).parent();
    lastKeyInsight.append(
        `<div class="key-insight-field row">
                        <div class="col-12">
                            <label for="KeyInsightsList_${numberOfKeyInsights}_">KeyInsightsList[${numberOfKeyInsights}]</label>
                        </div>
                        <div class="col-10" data-children-count="1">
                            <input class="form-control" type="text" id="KeyInsightsList_${numberOfKeyInsights}_" name="KeyInsightsList[${numberOfKeyInsights}]" value="">
                            <span class="text-danger field-validation-valid" data-valmsg-for="KeyInsightsList[${numberOfKeyInsights}]" data-valmsg-replace="true"></span>
                        </div>
                        <div class="col-2"><button onclick="removeField(this, 'keyInsight')">Delete</button></div>
                    </div>`
    );
}

function addSuggestedTagField(buttonRef) {
    const numberOfSuggestedTags = $(buttonRef).parent().find(".suggested-tag-field").length;
    const lastSuggestedTag = $(buttonRef).parent();
    lastSuggestedTag.append(
        `<div class="suggested-tag-field row">
                                <div class="col-12">
                                    <label for="SuggestedTagList_${numberOfSuggestedTags}_">SuggestedTagList[${numberOfSuggestedTags}]</label>
                                </div>
                                <div class="col-10" data-children-count="1">
                                    <input class="form-control" type="text" id="SuggestedTagList_${numberOfSuggestedTags}_" name="SuggestedTagList[${numberOfSuggestedTags}]" value="">
                                    <span class="text-danger field-validation-valid" data-valmsg-for="SuggestedTagList[${numberOfSuggestedTags}]" data-valmsg-replace="true"></span>
                                </div>
                                <div class="col-2"><button onclick="removeField(this, 'suggestedTag')">Delete</button></div>
                            </div>`
    );
    if (numberOfSuggestedTags == 0) {
        buttonRef.innerHTML = "Suggest another tag";
    }
}

function showField(button, fieldId) {
    $("#" + fieldId).show();
    $(button).hide();
}

function removeField(buttonRef, field) {
    console.log("thing");
    $(buttonRef).parent().parent().remove();
    if (field == "suggestedTag") {
        const numberOfSuggestedTags = $(".suggested-tag-field").length;
        if (numberOfSuggestedTags == 0) {
            $("#add-suggested-tag-button").html("Suggest a tag")
        }
    }
}

function hideField(fieldId, addButton) {
    $("#" + fieldId + " input").val('');
    $("#" + fieldId).hide();
    $("#" + addButton).show();
}

function handleFiles(fileUploadBox) {
    const fileList = fileUploadBox.files;

    //Save files to directory
    var form_data = new FormData();
    for (var i = 0; i < fileList.length; i++) {
        form_data.append(fileList[i].name, fileList[i]);
    }

    $.ajax({
        url: "/Item/PostFiles",
        cache: false,
        contentType: false,
        processData: false,
        data: form_data,
        type: 'post',
        success: function (data) {
            let newFileNameString = $("#file-names").val() + data;
            $("#file-names").val(newFileNameString);

            //Update file list html
            if (data) {
                let newFileNames = data.split(",");
                newFileNames.pop();
                let filesHtml = "";
                for (let i = 0; i < newFileNames.length; i++) {
                    let downloadLink = "/Item/GetDownloadedFile/?filePath=\\files\\documents\\items\\temp\\" + newFileNames[i];
                   filesHtml += `<div class="file"><a href="${downloadLink}"><h2>${newFileNames[i]}</h2></a><button onclick="removeFile(this)">Delete</button><br><br></div>`;
                };
                $("#files-list").append(filesHtml);
            }
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}

function removeFile(buttonRef) {
    const fileName = $(buttonRef).parent().find("h2").first().text();
    $.ajax({
        url: "/Item/DeleteFiles",
        contentType: "application/json; charset=utf-8",
        data: { 'name': fileName },
        type: 'GET',
        cache: false,
        success: function () {
            let newFileNameString = $("#file-names").val().replace(fileName + ",", '');
            $("#file-names").val(newFileNameString);
            console.log($("#file-names").val());
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
    $(buttonRef).parent().remove();
}

