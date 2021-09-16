$(document).ready(function () {
    $("#open-add-team-modal-button").click(function () {
        $("#addTeamModal").show();
        event.preventDefault();
    });

    $("#close-add-team-modal-button, .projectModalClose").click(function () {
        $("#addTeamModal").hide();
        $("#add-team-input").val("");
        event.preventDefault();
    });

    $("#add-team-submit-button").click(function () {
        const newTeamName = $("#add-team-input").val();
        console.log("thing");
        $.ajax({
            type: "GET",
            url: "/Team/SaveNewTeamName",
            data: { "teamName": newTeamName },
            contentType: "application/json; charset=utf-8",
        });
    });

    ////If user clicks outside the modal
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
    console.log(`.${type}-name-input`);
};

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
    console.log(`.${type}-name-input`);
};