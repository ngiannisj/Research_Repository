$(document).ready(function () {
    //When uploaders (not librarians) click the submit button on a 'Draft' item
    $("#submission-item-button").click(function () {
        $("#itemSubmissionConfirmModal").show();
        event.preventDefault();
    });

    //Open the 'profile' page when the item submission modal closes
    $("#close-submission-confirm-modal-button, submissionConfirmModalClose").click(function () {
        window.location.href = '/profile';
    });
    $(document).mouseup(function (e) {
        const modal = $("#itemSubmissionConfirmModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                window.location.href = '/profile';
            }
        }
    });

    //When a file is uploaded
    $("#file-upload-box").change(function () {
        handleFiles(this);
    });

    //Disable tag dropdown list if no theme is selected
    if ($("#theme-selector").val() != null && $("#theme-selector").val() != 0) {
        $("#tag-selector select").prop('disabled', false);
    }

    //Disable project dropdown list if no team is selected
    if ($("#team-selector").val()) {
        $("#project-selector").prop('disabled', false);
    }

    //Enable project dropdown list if a team is selected
    $("#team-selector").change(function () {
        if ($(this).val() != 0) {
            $("#project-selector").prop("selectedIndex", 0);
            $("#project-selector").prop('disabled', false);
        }
    });

    //Enable tag dropdown list if a theme is selected
    $("#theme-selector").change(function () {
        if ($(this).val() != 0) {
            $("#tag-selector select").prop('disabled', false);
        }
    });

    //If status of the item is set to rejected, show the comment textbox (To explain the reason for rejection)
    $("#status-selector").change(function () {
        if ($(this).val() == "Rejected") {
            $("#comment-input").show();
        } else {
            $("#comment-input").hide();
        }
    });

});

//Add a key insight field when add new insight field button is clicked
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
    event.preventDefault();
}

//Add a suggested tag field when add new tag field button is clicked
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
    event.preventDefault();
}

//Generic show field function
function showField(button, fieldId) {
    $("#" + fieldId).show();
    $(button).hide();
    event.preventDefault();
}

//Generic hide field function
function hideField(fieldId, addButton) {
    $("#" + fieldId + " input").val('');
    $("#" + fieldId).hide();
    $("#" + addButton).show();
    event.preventDefault();
}

//Generic remove field function
function removeField(buttonRef, field) {
    $(buttonRef).parent().parent().remove();
    if (field == "suggestedTag") {
        const numberOfSuggestedTags = $(".suggested-tag-field").length;
        if (numberOfSuggestedTags == 0) {
            $("#add-suggested-tag-button").html("Suggest a tag")
        }
    }
    event.preventDefault();
}

//Update custom files list
function handleFiles(fileUploadBox) {
    //Create custom form data object for file list
    let rawFileList = [];
    rawFileList = fileUploadBox.files;

    let filteredFileList = [];
    if (rawFileList.length > 0) {
        const allowedExtensions = ["pdf", "doc", "docx", "jpg", "png", "ppt"];
        for (let i = 0; i <= rawFileList.length - 1; i++) {
            let fileType = rawFileList[i].name.split('.').pop().toLowerCase();
            if (!allowedExtensions.filter(e => e.includes(fileType)).length) {
                alert(`File ${rawFileList[i].name} is not the correct type, files must have an extension of .pdf/.doc/.jpg/.png/.ppt only.`);
                return
            }

            //Check file size
            const fileSize = Math.round((rawFileList[i].size / 1024));
            if (fileSize >= 4096) {
                alert(
                    `File ${rawFileList[i].name} is too Big, please select a file less than 4mb`);
                return
            } else {
                filteredFileList.push(rawFileList[i]);
            }
        }
    }

    var form_data = new FormData();

    for (let i = 0; i < filteredFileList.length; i++) {
        form_data.append(filteredFileList[i].name, filteredFileList[i]);
    }

    //Post files to server
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
        }
    });
    event.preventDefault();
}

//Remove file from custom file list and delete from server
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
        },
        error: function (error) {
            console.log(error);
        }
    });
    $(buttonRef).parent().remove();
    event.preventDefault();
}

