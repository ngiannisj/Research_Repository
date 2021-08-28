$(document).ready(function () {
    $("#profile-edit-btn").click(function () {
        event.preventDefault();
        //Show inputs for user details
        $("#profile-save-btn").show();
        $("#user-role-input").show();
        $("#user-name-input").show();
        $("#user-team-selector").show();

        //Hide display text for user details
        $("#user-name-text").hide();
        $("#user-team-text").hide();
        $("#user-role-text").hide();
        $(this).hide();
    });

    $(".selected").click();
});

function filterProfileItems(status) {
    $(".item").hide();
    $(`.item[data-status='${status}']`).show();
    $(".selected").removeClass("selected");
    $(this).addClass("selected");
}