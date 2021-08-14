$(document).ready(function () {
    // Get the modal
    var modal = document.getElementById("myModal");

    // Get the button that opens the modal
    var btn = document.getElementById("myBtn");

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // When the user clicks the button, open the modal 
    btn.onclick = function () {
        event.preventDefault();
        $("#tag-delete-button").hide();
        $("#tag-submit-button").hide();
        $("#tag-delete-button").prop('disabled', true);
        $("#tag-submit-button").prop('disabled', true);
        modal.style.display = "block";
        saveState(getThemes());
    }

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        modal.style.display = "none";
        $("#tag-delete-button").show();
        $("#tag-submit-button").show();
        $("#tag-delete-button").prop('disabled', false);
        $("#tag-submit-button").prop('disabled', false);
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
            $("#tag-select-dropdown").val(0);
        }
    }

    $("#tag-select-dropdown").change(function () {
        populateTagNameField($(this));
    });

    $(".tag-modal-btn").click(function () {
        updateTags($(this));
    });
});

function getThemes() {
    let themesList = [];

    $(".theme").each(function (index, element) {
        let tags = [];
        let tagSelectList = [];

        const themeId = $(element).find(".theme-id").first().val();
        const themeName = $(element).find(".theme-name").first().val();
        const theme = {Id: themeId, Name: themeName, Image: ""}

        $(element).find(".tag-checkbox").each(function (i, e) {
            const tagId = $(e).find(".tag-checkbox-id").first().val();
            const tagName = $(e).find(".tag-checkbox-name").first().val();
            const tagStatus = $(e).find(".tag-checkbox-state").first().is(':checked');
            const tag = { TagId: tagId, Name: tagName, CheckedState: tagStatus }
            tags.push(tag);

            const tagSelectListItem = { Text: tagName, Value: tagId }
            tagSelectList.push(tagSelectListItem);
        })
        const themeVM = { Theme: theme, TagCheckboxes: tags, TagSelectList: tagSelectList }
        themesList.push(themeVM);
    });
    return themesList;
}

function saveState(themesList) {
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
            
            if (data == "newTag") {
                $("#tag-name-input").val("");
                $("#tag-submit-button").val("Add");
                $("#tag-delete-button").hide();
                $("#tag-delete-button").prop('disabled', true);
                $("#tag-submit-button").prop('disabled', false);
            } else {
                $("#tag-name-input").val(data);
                $("#tag-submit-button").val("Update");
                $("#tag-delete-button").show();
                $("#tag-submit-button").show();
                $("#tag-delete-button").prop('disabled', false);
                $("#tag-submit-button").prop('disabled', false);
            }
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}

//Update tags
function updateTags($this) {

    let selectedTagId = $this.closest(".modal-content").find("#tag-select-dropdown").first().find(":selected").attr("value");

    if (selectedTagId == "newTag") {
        selectedTagId = 0;
    } else {
        selectedTagId = parseInt(selectedTagId);
    }

    let tagName = $this.closest(".modal-content").find("#tag-name-input").first().val();

    let formAction = $this.val();

        $("#tag-select-dropdown").val(0);
        $("#tag-name-input").val("");

    $.ajax({
        type: "GET",
        url: "/Tag/UpdateTag",
        data: { "id": selectedTagId, "tagName": tagName, "actionName": formAction },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            var $el = $("#tag-select-dropdown");
            $('#tag-select-dropdown option:gt(1)').remove(); // remove all options, but not the first

            for (let i = 0; i < data.length; i++) {
                $el.append($("<option></option>").attr("value", data[i].id).text(data[i].name));
            }
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}