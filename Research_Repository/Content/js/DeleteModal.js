$(document).ready(function () {
    //Delete team modal
    $(".open-delete-team-modal-btn").click(function () {
        $("#deleteTeamModal").show();
        const teamId = $(this).val();
        $("#delete-team-confirm-btn").val(teamId);
        event.preventDefault();
    });

    $("#close-delete-team-modal-button, .teamModalClose").click(function () {
        $("#deleteTeamModal").hide();
        event.preventDefault();
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteTeamModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
            }
        }
    });

    //======================================================================================================

    //Delete project modal
    $("#open-project-delete-modal-btn").click(function () {
        $("#myProjectModal").hide();
        $("#deleteProjectModal").show();
        event.preventDefault();
    });

    $("#close-delete-project-modal-button, .projectModalClose").click(function () {
        $("#deleteProjectModal").hide();
        $("#myProjectModal").show();
        event.preventDefault();
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteProjectModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
            }
        }
    });

    //======================================================================================================

    //Delete theme modal
    $(".open-delete-theme-modal-btn").click(function () {
        $("#deleteThemeModal").show();
        const themeId = $(this).val();
        $("#delete-theme-confirm-btn").val(themeId);
        event.preventDefault();
    });

    $("#close-delete-theme-modal-button, .themeModalClose").click(function () {
        $("#deleteThemeModal").hide();
        event.preventDefault();
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteThemeModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
            }
        }
    });

    //======================================================================================================

    //Delete tag modal
    $("#open-delete-tag-modal-btn").click(function () {
        $("#myTagModal").hide();
        $("#deleteTagModal").show();
        event.preventDefault();
    });

    $("#close-delete-tag-modal-button, .tagModalClose, #tag-delete-button").click(function () {
        $("#deleteTagModal").hide();
        $("#myTagModal").show();
        event.preventDefault();
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteTagModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
            }
        }
    });

    //======================================================================================================

    //Delete item modal
    $("#open-delete-item-modal-btn").click(function () {
        showItemDeleteModal(this);
    });

    $("#close-delete-item-modal-button, .itemModalClose").click(function () {
        $("#deleteTagModal").hide();
        event.preventDefault();
    });

    $("#delete-item-modal-button").click(function () {
        const itemId = $(this).val();
        $.ajax({
            type: "GET",
            url: "/Item/Delete",
            data: { "id": itemId },
            contentType: 'application/json; charset=utf-8',
            success: function () {
                window.location.href = '../profile';
            }
        });
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteItemModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
            }
        }
    });

});

//This function exists for item links loaded through solr. Because they are loaded after page load.
function showItemDeleteModal($this) {
    $("#deleteItemModal").show();
    const itemId = $this.value;
    $("#delete-item-modal-button").val(itemId);
    event.preventDefault();
}