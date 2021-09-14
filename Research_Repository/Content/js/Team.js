$(document).ready(function () {
    //Team
    $(".edit-team-name-btn").click(function () {
        event.preventDefault();
        //Show
        $(this).siblings(".team-name-input").show();
        $(this).siblings(".apply-team-name-btn").show();

        //Hide
        $(this).siblings(".team-name-text").hide();
        $(this).hide();
    });

    $(".apply-team-name-btn").click(function () {
        event.preventDefault();
        //Change text value
        let newTeamName = $(this).siblings(".team-name-input").val();
        $(this).siblings(".team-name-text").html(newTeamName);

        //Show
        $(this).siblings(".team-name-text").show();
        $(this).siblings(".edit-team-name-btn").show();

        //Hide
        $(this).siblings(".team-name-input").hide();
        $(this).hide();
    });

    //Theme
    $(".edit-theme-name-btn").click(function () {
        event.preventDefault();
        //Show
        $(this).siblings(".theme-name-input").show();
        $(this).siblings(".apply-theme-name-btn").show();

        //Hide
        $(this).siblings(".theme-name-text").hide();
        $(this).hide();
    });

    $(".apply-theme-name-btn").click(function () {
        event.preventDefault();
        //Change text value
        let newTeamName = $(this).siblings(".theme-name-input").val();
        $(this).siblings(".theme-name-text").html(newTeamName);

        //Show
        $(this).siblings(".theme-name-text").show();
        $(this).siblings(".edit-theme-name-btn").show();

        //Hide
        $(this).siblings(".theme-name-input").hide();
        $(this).hide();
    });
});