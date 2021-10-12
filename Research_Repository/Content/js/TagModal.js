$(document).ready(function () {

    // When the user clicks the open button, open the modal 
    $("#myTagBtn").click(function () {
        $("#tag-name-input-container").addClass("hidden");
        $("#open-delete-tag-modal-btn-container").addClass("hidden");
        $("#tag-submit-button-container").addClass("hidden");
        $("#open-delete-tag-modal-btn").prop('disabled', true);
        $("#tag-submit-button").prop('disabled', true);
        $("#myTagModal").removeClass("hidden");
        saveTempThemes(getThemes());
        $("body").addClass("no-scroll");
        findInsiders($("#myTagModal"));
        event.preventDefault();
    });

     //======================================================================
    //These ajax calls are here because there is too much data to pass through the url without compromising security

    //Delete theme from session
    $("#delete-theme-confirm-btn").click(function () {
        const themeId = $(this).val();
        const themesList = getThemes();
        const jsonThemesList = JSON.stringify(themesList);
        $.ajax({
            type: "POST",
            url: "/Theme/DeleteTheme",
            data: { "themeVMString": jsonThemesList, "deleteId": themeId },
            success: function (data) {
                window.location.replace('../theme?redirect=True');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    $($("#selected-theme-name-input")).on("input", function () {
        if ($(this).hasClass("error")) {
            $("#selected-theme-name-error-text").addClass("hidden");
            $("#selected-theme-name-input").removeClass("error");
        }
    });
        //Add theme to session
    $("#add-theme-submit-button").click(function () {
        event.preventDefault();
        const themesList = getThemes();
        const jsonThemesList = JSON.stringify(themesList);
        const name = $("#selected-theme-name-input").val();
        const desc = $("#selected-theme-description-input").val();

        if (!$("#selected-theme-name-input").val()) {
            $("#selected-theme-name-error-text").removeClass("hidden");
            $("#selected-theme-name-input").addClass("error");
            return
        }

        $.ajax({
            type: "POST",
            url: "/Theme/AddTheme",
            data: { "themeVMString": jsonThemesList, "themeName": name, "themeDesc": desc },
            success: function (data) {
                window.location.replace('../theme?redirect=True');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    //Save themes to database
    $("#save-themes-button").click(function () {
        event.preventDefault();
        const themesList = getThemes();
        const jsonThemesList = JSON.stringify(themesList);
        $.ajax({
            type: "POST",
            url: "/Theme/SaveThemes",
            data: { "themeVMString": jsonThemesList },
            success: function (data) {
                window.location.replace('../theme');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
    //=====================================================

    // When the user clicks on close button, close the modal
    $("#close-tag-modal-button").click(function () {
        event.preventDefault();
        $("#myTagModal").addClass("hidden");
        $("#tag-name-input").val("");
        $("#tag-name-input-container").addClass("hidden");
        $("#tagId-modal-input").val("");
        $("#tagId-modal-selectList").html("");
        $("#open-delete-tag-modal-btn-container").addClass("hidden");
        $("#tag-submit-button-container").addClass("hidden");
        $("#open-delete-tag-modal-btn").prop('disabled', true);
        $("#tag-submit-button").prop('disabled', true);
        $("#tag-name-error-text").addClass("hidden");
        $("#tag-name-input").removeClass("error");
        updateThemeTags();
        $("body").removeClass("no-scroll");
    })

    //If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#myTagModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("#tag-name-input").val("");
                $("#tag-name-input-container").addClass("hidden");
                $("#tagId-modal-input").val("");
                $("#tagId-modal-selectList").html("");
                $("#open-delete-tag-modal-btn-container").addClass("hidden");
                $("#tag-submit-button-container").addClass("hidden");
                $("#open-delete-tag-modal-btn").prop('disabled', true);
                $("#tag-submit-button").prop('disabled', true);
                $("#tag-name-error-text").addClass("hidden");
                $("#tag-name-input").removeClass("error");
                updateThemeTags();
                $("body").removeClass("no-scroll");
            }
        }

    });
    //On tag select list dropdown change, set tag name field
    $(".tag-id-option").click(function () {
        if ($(this).data("value") != $("#tagId-modal-input").val()) {
            populateTagNameField($(this));
            $("#check-all-status").prop("checked", false);
            $("#check-all-status-container").removeClass("hidden");
            $("#tag-name-input-container").removeClass("hidden");
        }
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

$($("#tag-name-input")).on("input", function () {
    if ($(this).hasClass("error")) {
        $("#tag-name-error-text").addClass("hidden");
        $("#tag-name-input").removeClass("error");
    }
});

//Update tag name field based on tag selected from dropdown
function populateTagNameField($this) {
    let selectedTagId = $($this).data("value");
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
                $("#open-delete-tag-modal-btn-container").addClass("hidden");
                $("#open-delete-tag-modal-btn").prop('disabled', true);
            } else {
                $("#tag-name-input").val(data);
                $("#tag-submit-button").val("Update");
                $("#open-delete-tag-modal-btn-container").removeClass("hidden");
                $("#open-delete-tag-modal-btn").prop('disabled', false);
            }
            $("#tag-name-input-container").removeClass("hidden");
            $("#tag-submit-button-container").removeClass("hidden");
            $("#tag-submit-button").prop('disabled', false);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Update tags
function updateTags($this) {

    if (!$("#tag-name-input").val()) {
        $("#tag-name-error-text").removeClass("hidden");
        $("#tag-name-input").addClass("error");
        return
    }

    let selectedTagId = $("#tagId-modal-input").val();

    if (selectedTagId == "newTag") {
        selectedTagId = 0;
    } else {
        selectedTagId = parseInt(selectedTagId);
    }

    let tagName = $("#tag-name-input").first().val();

    let checkAllStatus = $("#check-all-status").prop("checked");

    let formAction = $this.val();

    $("#tagId-modal-input").val("");
    $("#tagId-modal-selectList").html("");
    $("#tag-name-input").val("");

    $.ajax({
        type: "GET",
        url: "/Tag/UpdateTag",
        data: { "id": selectedTagId, "tagName": tagName, "checkAll": checkAllStatus, "actionName": formAction },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            //Reload tag dropdown list
            let $el = $(".accordion__content--select-list").first();
            $('#tagId-modal-input').html(""); // remove all options, but not the first two
            let options = "";
            options += `<button onclick="selectListOptionClick(this)"
                                    class="accordion__content-option"
                                    data-value="newTag">New Tag
                            </button>`
            for (let i = 0; i < data.length; i++) {
                options += `<button onclick="selectListOptionClick(this)"
                                    class="accordion__content-option"
                                    data-value="${data[i].id}">${data[i].name}
                            </button>`
            }

            $($el).html(options);

            //Hide and uncheck 'checkAll' checkbox
            $("#check-all-status").prop("checked", false);
            $("#check-all-status-container").addClass("hidden");
            $("#check-all-status-container .field__label--checkbox").removeClass("field__label--checkbox-checked");
            $("#tag-name-input-container").addClass("hidden");

            if (!$("#open-delete-tag-modal-btn-container").hasClass("hidden")) {
                $("#open-delete-tag-modal-btn-container").addClass("hidden");
            }
            $("#tag-submit-button-container").addClass("hidden");
            $("#tag-name-input").val("");
            $("#tag-name-input-container").addClass("hidden");
            $("#tagId-modal-input").val("");
            $("#tagId-modal-selectList").html("");
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