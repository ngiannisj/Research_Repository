////$(document).ready(function () {
////    // Get the modal
////    var tagModal = document.getElementById("myTagModal");

////    // Get the button that opens the modal
////    var tagModalOpenBtn = document.getElementById("myTagBtn");

////    // Get the button element that closes the modal
////    var tagModalCloseBtn = document.getElementsByClassName("tagModalClose")[0];

////    // When the user clicks the open button, open the modal 
////    tagModalOpenBtn.onclick = function () {
////        event.preventDefault();
////        $("#tag-name-input").hide();
////        $("#tag-delete-button").hide();
////        $("#tag-submit-button").hide();
////        $("#tag-delete-button").prop('disabled', true);
////        $("#tag-submit-button").prop('disabled', true);
////        tagModal.style.display = "block";
////        saveState(getThemes());
////    }

////    // When the user clicks on close button, close the modal
////    tagModalCloseBtn.onclick = function () {
////        tagModal.style.display = "none";
////        $("#tag-name-input").val("");
////        $("#tag-name-input").hide();
////        $("#tag-select-dropdown").val(0);
////        $("#tag-delete-button").hide();
////        $("#tag-submit-button").hide();
////        $("#tag-delete-button").prop('disabled', true);
////        $("#tag-submit-button").prop('disabled', true);
////        updateThemeTags();
////    }

////    // When the user clicks anywhere outside of the modal, close it
////    window.onclick = function (event) {
////        if (event.target == tagModal) {
////            tagModal.style.display = "none";
////            $("#tag-name-input").val("");
////            $("#tag-name-input").hide();
////            $("#tag-select-dropdown").val(0);
////            $("#tag-delete-button").hide();
////            $("#tag-submit-button").hide();
////            $("#tag-delete-button").prop('disabled', true);
////            $("#tag-submit-button").prop('disabled', true);
////            updateThemeTags();
////        }
////    }

////    $("#tag-select-dropdown").change(function () {
////        populateTagNameField($(this));
////    });

////    $(".tag-modal-btn").click(function () {
////        updateTags($(this));
////    });
////});

////function getThemes() {
////    let themesList = [];

////    $(".theme").each(function (index, element) {
////        let tags = [];
////        let tagSelectList = [];

////        const themeId = $(element).find(".theme-id").first().val();
////        const themeName = $(element).find(".theme-name").first().val();
////        const theme = {Id: themeId, Name: themeName, Image: ""}

////        $(element).find(".tag-checkbox").each(function (i, e) {
////            const tagId = $(e).find(".tag-checkbox-id").first().val();
////            const tagName = $(e).find(".tag-checkbox-name").first().val();
////            const tagStatus = $(e).find(".tag-checkbox-state").first().is(':checked');
////            const tag = { TagId: tagId, Name: tagName, CheckedState: tagStatus }
////            tags.push(tag);

////            const tagSelectListItem = { Text: tagName, Value: tagId }
////            tagSelectList.push(tagSelectListItem);
////        })
////        const themeVM = { Theme: theme, TagCheckboxes: tags, TagSelectList: tagSelectList }
////        themesList.push(themeVM);
////    });
////    return themesList;
////}

////function saveState(themesList) {
////    let tempThemes = JSON.stringify(themesList);
////    $.ajax({
////        type: "POST",
////        url: "/Theme/SaveThemesState",
////        traditional: true,
////        data: tempThemes,
////        contentType: 'application/json; charset=utf-8',
////        dataType: 'json'
////    });
////}

//////Update tag name field based on tag selected from dropdown
////function populateTagNameField($this) {

////    let selectedTagId = parseInt($this.find(":selected").attr("value"));

////    if (selectedTagId == "newTag") {
////        selectedTagId = null;
////    }

////    $.ajax({
////        type: "GET",
////        url: "/Tag/GetTagName",
////        data: { "id": selectedTagId },
////        contentType: 'application/json; charset=utf-8',
////        success: function (data) {
            
////            if (data == "newTag") {
////                $("#tag-name-input").val("");
////                $("#tag-submit-button").val("Add");
////                $("#tag-delete-button").hide();
////                $("#tag-delete-button").prop('disabled', true);
////            } else {
////                $("#tag-name-input").val(data);
////                $("#tag-submit-button").val("Update");
////                $("#tag-delete-button").show();
////                $("#tag-delete-button").prop('disabled', false);
////            }
////            $("#tag-name-input").show();
////            $("#tag-submit-button").show();
////            $("#tag-submit-button").prop('disabled', false);
////        },
////        error: function (error) {
////            console.log(error);
////            alert("An error occurred!!!")
////        }
////    });
////}

//////Update tags
////function updateTags($this) {

////    let selectedTagId = $this.closest(".modal-content").find("#tag-select-dropdown").first().find(":selected").attr("value");

////    if (selectedTagId == "newTag") {
////        selectedTagId = 0;
////    } else {
////        selectedTagId = parseInt(selectedTagId);
////    }

////    let tagName = $this.closest(".modal-content").find("#tag-name-input").first().val();

////    let formAction = $this.val();

////        $("#tag-select-dropdown").val(0);
////        $("#tag-name-input").val("");

////    $.ajax({
////        type: "GET",
////        url: "/Tag/UpdateTag",
////        data: { "id": selectedTagId, "tagName": tagName, "actionName": formAction },
////        contentType: 'application/json; charset=utf-8',
////        success: function (data) {
////            var $el = $("#tag-select-dropdown");
////            $('#tag-select-dropdown option:gt(1)').remove(); // remove all options, but not the first

////            for (let i = 0; i < data.length; i++) {
////                $el.append($("<option></option>").attr("value", data[i].id).text(data[i].name));
////            }
////        },
////        error: function (error) {
////            console.log(error);
////            alert("An error occurred!!!")
////        }
////    });
////}

////function updateThemeTags() {
////    $.ajax({
////        type: "GET",
////        url: "/Theme/UpdateThemeTags",
////        contentType: 'application/json; charset=utf-8',
////        success: function (data) {
////            window.location.replace('../theme?redirect=True');
////        },
////        error: function (error) {
////            console.log(error);
////            alert("An error occurred!!!")
////        }
////    });
////}