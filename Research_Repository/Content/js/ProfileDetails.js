$(document).ready(function () {
    //On edit profile button click
    $("#profile-edit-btn").click(function () {

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

        event.preventDefault();
    });

    //Click selected item filter on initial load to load solr items
    $(".selected").first().click();
});