$(document).ready(function () {
    //On edit profile button click
    $("#profile-edit-btn").click(function () {

        //Show inputs for user details
        $("#profile-save-btn").removeClass("hidden");
        $("#user-role-input").removeClass("hidden");
        $("#user-firstName-input").removeClass("hidden");
        $("#user-lastName-input").removeClass("hidden");
        $("#user-email-input").removeClass("hidden");
        $("#user-team-selector").removeClass("hidden");

        //Hide display text for user details
        $("#user-firstName-text").addClass("hidden");
        $("#user-lastName-text").addClass("hidden");
        $("#user-email-text").addClass("hidden");
        $("#user-team-text").addClass("hidden");
        $("#user-role-text").addClass("hidden");
        $(this).addClass("hidden");

        event.preventDefault();
    });

    //Click selected item filter on initial load to load solr items
    $(".selected").first().click();
});