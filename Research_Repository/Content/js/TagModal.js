$(document).ready(function () {

    // When the user clicks the open button, open the modal 
    $("#myTagBtn").click(function () {
        $("#tag-name-input").hide();
        $("#open-delete-tag-modal-btn").hide();
        $("#tag-submit-button").hide();
        $("#open-delete-tag-modal-btn").prop('disabled', true);
        $("#tag-submit-button").prop('disabled', true);
        $("#myTagModal").show();
        saveTempThemes(getThemes());

        event.preventDefault();
    });

    // When the user clicks on close button, close the modal
    $(".tagModalClose").click(function () {
        $("#myTagModal").hide();
        $("#tag-name-input").val("");
        $("#tag-name-input").hide();
        $("#tag-select-dropdown").val(0);
        $("#open-delete-tag-modal-btn").hide();
        $("#tag-submit-button").hide();
        $("#open-delete-tag-modal-btn").prop('disabled', true);
        $("#tag-submit-button").prop('disabled', true);
        updateThemeTags();
    })

    //If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#myTagModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
                $("#tag-name-input").val("");
                $("#tag-name-input").hide();
                $("#tag-select-dropdown").val(0);
                $("#open-delete-tag-modal-btn").hide();
                $("#tag-submit-button").hide();
                $("#open-delete-tag-modal-btn").prop('disabled', true);
                $("#tag-submit-button").prop('disabled', true);
                updateThemeTags();
            }
        }

    });
    //On tag select list dropdown change, set tag name field
    $("#tag-select-dropdown").change(function () {
        populateTagNameField($(this));
        $("#check-all-status").prop("checked", false);
        $("#check-all-status-container").show();
        $("#tag-name-input-container").show();
    });

    //On tag modal btn click, update tags
    $(".tag-modal-btn").click(function () {
        updateTags($(this));
    });
});

//Get list of themes from DOM
function getThemes() {
    let themesList = [];

    $(".theme").each(function (index, element) {
        let tags = [];
        let tagSelectList = [];

        const themeId = $(element).find(".theme-id").first().val();
        const themeName = $(element).find(".theme-name-input").first().val();
        const themeDescription = $(element).find(".theme-description-input").first().val();
        const theme = { Id: themeId, Name: themeName, Description: themeDescription }

        $(element).find(".tag-checkbox").each(function (i, e) {
            const tagId = $(e).find(".tag-checkbox-id").first().val();
            const tagName = $(e).find(".tag-checkbox-name").first().val();
            const tagStatus = $(e).find(".tag-checkbox-state").first().is(':checked');
            const tag = { Value: tagId, Name: tagName, CheckedState: tagStatus }
            tags.push(tag);

            const tagSelectListItem = { Text: tagName, Value: tagId }
            tagSelectList.push(tagSelectListItem);
        })
        const themeVM = { Theme: theme, TagCheckboxes: tags, TagSelectList: tagSelectList }
        themesList.push(themeVM);
    });
    return themesList;
}

//Save temp themes to session
function saveTempThemes(themesList) {
    let tempThemes = JSON.stringify(themesList);
    $.ajax({
        type: "POST",
        url: "/Theme/SaveThemesState",
        traditional: true,
        data: tempThemes,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}

//Update tag name field based on tag selected from dropdown
function populateTagNameField($this) {
    let selectedTagId = parseInt($this.find(":selected").attr("value"));
    if (selectedTagId == "newTag") {
        selectedTagId = null;
    }
    $.ajax({
        type: "GET",
        url: "/Tag/GetTagName",
        data: { "id": selectedTagId },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            //Show/Hide button in modal based on dropdown selection
            if (data == "newTag") {
                $("#tag-name-input").val("");
                $("#tag-submit-button").val("Add");
                $("#open-delete-tag-modal-btn").hide();
                $("#open-delete-tag-modal-btn").prop('disabled', true);
            } else {
                $("#tag-name-input").val(data);
                $("#tag-submit-button").val("Update");
                $("#open-delete-tag-modal-btn").show();
                $("#open-delete-tag-modal-btn").prop('disabled', false);
            }
            $("#tag-name-input").show();
            $("#tag-submit-button").show();
            $("#tag-submit-button").prop('disabled', false);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Update tags
function updateTags($this) {

    let selectedTagId = $("#tag-select-dropdown").first().find(":selected").attr("value");

    if (selectedTagId == "newTag") {
        selectedTagId = 0;
    } else {
        selectedTagId = parseInt(selectedTagId);
    }

    let tagName = $("#tag-name-input").first().val();

    let checkAllStatus = $("#check-all-status").prop("checked");

    let formAction = $this.val();

    $("#tag-select-dropdown").val(0);
    $("#tag-name-input").val("");

    $.ajax({
        type: "GET",
        url: "/Tag/UpdateTag",
        data: { "id": selectedTagId, "tagName": tagName, "checkAll": checkAllStatus, "actionName": formAction },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            //Reload tag dropdown list
            var $el = $("#tag-select-dropdown");
            $('#tag-select-dropdown option:gt(1)').remove(); // remove all options, but not the first two

            for (let i = 0; i < data.length; i++) {
                $el.append($("<option></option>").attr("value", data[i].id).text(data[i].name));
            }

            //Hide and uncheck 'checkAll' checkbox
            $("#check-all-status").prop("checked", false);
            $("#check-all-status-container").hide();
            $("#tag-name-input-container").hide();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Reload theme page
function updateThemeTags() {
    window.location.replace('../theme?redirect=True');
}