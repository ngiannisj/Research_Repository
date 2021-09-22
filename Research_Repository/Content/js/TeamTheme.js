$(document).ready(function () {
    //Add team modal
    $("#open-add-team-modal-button").click(function () {
        $("#addTeamModal").show();
        event.preventDefault();
    });

    //On modal close
    $("#close-add-team-modal-button, .teamModalClose").click(function () {
        $("#addTeamModal").hide();
        $("#add-team-input").val("");
        event.preventDefault();
    });

    //If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#addTeamModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
                $("#add-team-input").val("");
            }
        }
    });

    //===================================================================================

    //Add theme modal
    $("#open-add-theme-modal-button").click(function () {
        $("#addThemeModal").show();
        event.preventDefault();
    });

    //On close theme modal
    $("#close-add-theme-modal-button, .themeModalClose").click(function () {
        $("#addThemeModal").hide();
        $("#add-theme-input").val("");
        event.preventDefault();
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#addThemeModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
                $("#add-theme-input").val("");
            }
        }
    });

})

//Edits name of teams and themes
function editName(thisbtn, type) {
    event.preventDefault();
    //Show
    $(thisbtn).siblings(`.${type}-name-input`).show();
    $(thisbtn).siblings(`.apply-${type}-name-btn`).show();

    //Hide
    $(thisbtn).siblings(`.${type}-name-text`).hide();
    $(thisbtn).hide();
};

//Apply name of new team and theme
function applyName(thisbtn, type) {
    event.preventDefault();
    //Change text value
    let newTeamName = $(thisbtn).siblings(`.${type}-name-input`).val();
    $(thisbtn).siblings(`.${type}-name-text`).html(newTeamName);

    //Show
    $(thisbtn).siblings(`.${type}-name-text`).show();
    $(thisbtn).siblings(`.edit-${type}-name-btn`).show();

    //Hide
    $(thisbtn).siblings(`.${type}-name-input`).hide();
    $(thisbtn).hide();
};