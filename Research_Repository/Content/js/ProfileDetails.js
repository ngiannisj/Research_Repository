$(document).ready(function () {
    $("#profile-edit-btn").click(function () {
        event.preventDefault();
        //Show inputs for user details
        $("#profile-save-btn").show();
        $("#user-role-input").show();
        $("#user-firstName-input").show();
        $("#user-lastName-input").show();
        $("#user-team-selector").show();

        //Hide display text for user details
        $("#user-firstName-text").hide();
        $("#user-lastName-text").hide();
        $("#user-team-text").hide();
        $("#user-role-text").hide();
        $(this).hide();
    });

    $(".selected").first().click();
});

function filterProfileItems(status) {
    $(".item").hide();
    $(`.item[data-status='${status}']`).show();
    $(".selected").removeClass("selected");
    $(this).addClass("selected");
}