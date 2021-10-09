$(document).ready(function () {
    //Delete team modal
    $(".open-delete-team-modal-btn").click(function () {
        $("#deleteTeamModal").removeClass("hidden");
        const teamId = $(this).val();
        $("#delete-team-confirm-btn").val(teamId);
        $("body").addClass("no-scroll");
        findInsiders($("#deleteTeamModal"));
        event.preventDefault();
    });

    $("#close-delete-team-modal-button, .teamModalClose").click(function () {
        $("#deleteTeamModal").addClass("hidden");
        event.preventDefault();
        $("body").removeClass("no-scroll");
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteTeamModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
            }
        }
    });

    //======================================================================================================

    //Delete project modal
    $("#open-project-delete-modal-btn").click(function () {
        $("#myProjectModal").addClass("hidden");
        $("#deleteProjectModal").removeClass("hidden");
        event.preventDefault();
        $("body").addClass("no-scroll");
        findInsiders($("#deleteProjectModal"));
    });

    $("#close-delete-project-modal-button, .projectModalClose").click(function () {
        $("#deleteProjectModal").addClass("hidden");
        $("#myProjectModal").removeClass("hidden");
        event.preventDefault();
        $("body").removeClass("no-scroll");
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteProjectModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
            }
        }
    });

    //======================================================================================================

    //Delete theme modal
    $(".open-delete-theme-modal-btn").click(function () {
        $("#deleteThemeModal").removeClass("hidden");
        const themeId = $(this).val();
        $("#delete-theme-confirm-btn").val(themeId);
        event.preventDefault();
        $("body").addClass("no-scroll");
        findInsiders($("#deleteThemeModal"));
    });

    $("#close-delete-theme-modal-button, .themeModalClose").click(function () {
        $("#deleteThemeModal").addClass("hidden");
        event.preventDefault();
        $("body").removeClass("no-scroll");
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteThemeModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
            }
        }
    });

    //======================================================================================================

    //Delete tag modal
    $("#open-delete-tag-modal-btn").click(function () {
        $("#myTagModal").addClass("hidden");
        $("#deleteTagModal").removeClass("hidden");
        event.preventDefault();
        $("body").addClass("no-scroll");
    });

    $("#close-delete-tag-modal-button, .tagModalClose, #tag-delete-button").click(function () {
        $("#deleteTagModal").addClass("hidden");
        $("#myTagModal").removeClass("hidden");
        event.preventDefault();
        $("body").removeClass("no-scroll");
        findInsiders($("#deleteTagModal"));
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteTagModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
            }
        }
    });

    //======================================================================================================

    //Delete item modal
    $("#open-delete-item-modal-btn").click(function () {
        showItemDeleteModal(this);
    });

    $("#close-delete-item-modal-button, .itemModalClose").click(function () {
        $("#deleteItemModal").addClass("hidden");
        event.preventDefault();
        $("body").removeClass("no-scroll");
    });

    $("#delete-item-modal-button").click(function () {
        const itemId = $(this).val();
        $.ajax({
            type: "GET",
            url: "/Item/Delete",
            data: { "id": itemId },
            contentType: 'application/json; charset=utf-8',
            success: function () {
                window.location.href = '/profile';
            }
        });
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#deleteItemModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
            }
        }
    });

});

//This function exists for item links loaded through solr. Because they are loaded after page load.
function showItemDeleteModal($this) {
    $("#deleteItemModal").removeClass("hidden");
    const itemId = $this.value;
    $("#delete-item-modal-button").val(itemId);
    event.preventDefault();
    $("body").addClass("no-scroll");
    findInsiders($("#deleteItemModal"));
}