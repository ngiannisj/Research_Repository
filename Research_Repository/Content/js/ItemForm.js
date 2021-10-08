$(document).ready(function () {

    $("#project-start-date.error").on("change", function () {
        if ($("#project-start-date").val() < $("#project-end-date").val()) {
            $("#project-start-date").removeClass("error");
            $("#end-date-error-text").addClass("hidden");
        }
    });

    $("#title-input-field.error").on("input", function () {
        $("#title-input-field").removeClass("error");
        $("#title-error-text").addClass("hidden");
    });

    $('#item-form').submit(function () {
        if ($("#project-start-date").val() >= $("#project-end-date").val()) {
            $("#project-end-date").addClass("error");
            $("#end-date-error-text").removeClass("hidden");

            if ($("#title-input-field").val() == null || $("#title-input-field").val() == "") {
                $("#title-input-field").addClass("error");
                $("#title-error-text").removeClass("hidden");
            }
            return false;
        }
        if ($("#title-input-field").val() == null || $("#title-input-field").val() == "") {
            $("#title-input-field").addClass("error");
            $("#title-error-text").removeClass("hidden");
            return false;
        }
    });

    //When uploaders (not librarians) click the submit button on a 'Draft' item
    $("#submission-item-button").click(function () {
        $("#itemSubmissionConfirmModal").removeClass("hidden");
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
        $("#tag-selector .field__input-checkbox").prop('disabled', false);
        $("#tag-selector .field__label--checkbox").removeClass("disabled");
    }

    //Disable project dropdown list if no team is selected
    if ($("#team-selector").val()) {
        $("#item-project-selectList").prop('disabled', false);
    }

    //Enable project dropdown list if a team is selected
    $("#item-team-selectList-container .accordion__content-option").click(function () {
        $("#project-selector").val("");
        $("#item-project-selectList").prop('disabled', false);
        $("#item-project-selectList").html("");
    });

    //Enable tag dropdown list if a theme is selected
    $("#item-theme-selectList-container .accordion__content-option").click(function () {
        $("#tag-selector .field__input-checkbox").prop('disabled', false);
        $("#tag-selector .field__label--checkbox").removeClass("disabled");
    });

    //If status of the item is set to rejected, show the comment textbox (To explain the reason for rejection)
    $("#status-selectList-container .accordion__content-option").click(function () {
        if ($("#status-selector").val() == "Rejected") {
            $("#comment-input").removeClass("hidden");
        } else {
            $("#comment-input").addClass("hidden");
        }
    });

});

//Add a key insight field when add new insight field button is clicked
function addKeyInsightField(buttonRef) {
    const numberOfKeyInsights = $(".key-insight-field").length;
    const lastKeyInsight = $("#key-insights");
    lastKeyInsight.append(
        `<div class="field key-insight-field">
    <label for="KeyInsightsList_${numberOfKeyInsights}_" class="field__label">Key insight</label>
    <input type="text"
id="KeyInsightsList_${numberOfKeyInsights}_"
           name="KeyInsightsList[${numberOfKeyInsights}]"
           class="field__input" />
    <div class="button-group">
        <div class="button-group__item field-modifier">
            <button class="link link--normal" onclick="removeField(this, 'keyInsight')">
                Remove<div class="link__icon">
                    <svg width="17" height="5" viewBox="0 0 17 5" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M15.625 0.71875H1.375C0.719277 0.71875 0.1875 1.25053 0.1875 1.90625V3.09375C0.1875 3.74947 0.719277 4.28125 1.375 4.28125H15.625C16.2807 4.28125 16.8125 3.74947 16.8125 3.09375V1.90625C16.8125 1.25053 16.2807 0.71875 15.625 0.71875Z" fill="#253C78" />
                    </svg>
                </div>
            </button>
        </div>
    </div>
</div>`



    );
    event.preventDefault();
}

//Add a suggested tag field when add new tag field button is clicked
function addSuggestedTagField(buttonRef) {
    const numberOfSuggestedTags = $(".suggested-tag-field").length;
    const lastSuggestedTag = $("#suggested-tags");
    lastSuggestedTag.append(
        `<div class="field suggested-field">
                            <label for="SuggestedTagList_${numberOfSuggestedTags}_" class="field__label">Suggested tag</label>
                            <input type="text"
                                   class="field__input"
                                    id="SuggestedTagList_${numberOfSuggestedTags}_"
                                   name="SuggestedTagList[${numberOfSuggestedTags}]"
                                    value=""/>
                            <div class="button-group">
                                <div class="button-group__item field-modifier">
                                    <button class="link link--normal" onclick="removeField(this, 'suggestedTag')">
                                        Remove<div class="link__icon">
                                            <svg width="17" height="5" viewBox="0 0 17 5" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M15.625 0.71875H1.375C0.719277 0.71875 0.1875 1.25053 0.1875 1.90625V3.09375C0.1875 3.74947 0.719277 4.28125 1.375 4.28125H15.625C16.2807 4.28125 16.8125 3.74947 16.8125 3.09375V1.90625C16.8125 1.25053 16.2807 0.71875 15.625 0.71875Z" fill="#253C78" />
                                            </svg>
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>`


    );
    if (numberOfSuggestedTags == 0) {
        buttonRef.innerHTML = `Suggest another tag <div class="link__icon">
                            <svg width="17" height="17" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M15.1268 6.4874H9.94315V1.30377C9.94315 0.667698 9.4273 0.151855 8.79123 0.151855H7.63931C7.00324 0.151855 6.4874 0.667698 6.4874 1.30377V6.4874H1.30377C0.667698 6.4874 0.151855 7.00324 0.151855 7.63931V8.79123C0.151855 9.4273 0.667698 9.94315 1.30377 9.94315H6.4874V15.1268C6.4874 15.7628 7.00324 16.2787 7.63931 16.2787H8.79123C9.4273 16.2787 9.94315 15.7628 9.94315 15.1268V9.94315H15.1268C15.7628 9.94315 16.2787 9.4273 16.2787 8.79123V7.63931C16.2787 7.00324 15.7628 6.4874 15.1268 6.4874Z" fill="#253C78"></path>
                            </svg>
                        </div>`;
    }
    event.preventDefault();
}

//Generic show field function
function showField(button, fieldId) {
    $("#" + fieldId).removeClass("hidden");
    $(button).addClass("hidden");
    event.preventDefault();
}

//Generic hide field function
function hideField(fieldId, addButton) {
    $("#" + fieldId + " input").val('');
    $("#" + fieldId).addClass("hidden");
    $("#" + addButton).removeClass("hidden");
    event.preventDefault();
}

//Generic remove field function
function removeField(buttonRef, field) {
    $(buttonRef).closest(".suggested-tag-field").remove();
    $(buttonRef).closest(".key-insight-field").remove();

    if (field == "suggestedTag") {
        const numberOfSuggestedTags = $(".suggested-tag-field").length;
        if (numberOfSuggestedTags == 0) {
            $("#add-suggested-tag-button").html(`Suggest a tag <div class="link__icon">
                            <svg width="17" height="17" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M15.1268 6.4874H9.94315V1.30377C9.94315 0.667698 9.4273 0.151855 8.79123 0.151855H7.63931C7.00324 0.151855 6.4874 0.667698 6.4874 1.30377V6.4874H1.30377C0.667698 6.4874 0.151855 7.00324 0.151855 7.63931V8.79123C0.151855 9.4273 0.667698 9.94315 1.30377 9.94315H6.4874V15.1268C6.4874 15.7628 7.00324 16.2787 7.63931 16.2787H8.79123C9.4273 16.2787 9.94315 15.7628 9.94315 15.1268V9.94315H15.1268C15.7628 9.94315 16.2787 9.4273 16.2787 8.79123V7.63931C16.2787 7.00324 15.7628 6.4874 15.1268 6.4874Z" fill="#253C78"></path>
                            </svg>
                        </div>`)
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
                    filesHtml +=
                        `<div class="field">
                        <div class="field__input field__input-file">
                            <a href="${downloadLink}" class="file-name link">${newFileNames[i]}</a>
                            <button onclick="removeFile(this)" class="button--icon button--icon-delete align-right">
                                <img src="../images/svgs/cross.svg" alt="remove_file">
                            </button>
                        </div>
                    </div>`;
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
    event.stopPropagation();
    const fileName = $(buttonRef).parent().find(".file-name").first().text();
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
    $(buttonRef).closest(".field").remove();
    event.preventDefault();
}

function clickFileUpload() {
    $("#file-upload-box").click();
    event.preventDefault();
}
