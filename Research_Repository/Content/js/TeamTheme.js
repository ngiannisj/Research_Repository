$(document).ready(function () {
    //Add team modal
    $("#open-add-team-modal-button").click(function () {
        $("#add-team-submit-button").show();
        $("#edit-team-submit-button").hide();
        $("#teamModal").show();
        $("#selected-team-name-input").val("");
        $("#selected-team-contact-input").val("");

        event.preventDefault();
    });

    //Save edited team
    $("#edit-team-submit-button").click(function () {
        $(".team--selected .team-name-text").html($("#selected-team-name-input").val());

        $(".team--selected .team-name-input").val($("#selected-team-name-input").val());
        $(".team--selected .team-contact-input").val($("#selected-team-contact-input").val());

        $("#selected-team-name-input").val("");
        $("#selected-team-contact-input").val("");
        $(".team--selected").removeClass("team--selected");
        $("#teamModal").hide();
        event.preventDefault();
    });

    //On modal close
    $("#close-add-team-modal-button, .teamModalClose").click(function () {
        $("#teamModal").hide();
        $("#selected-team-name-input").val("");
        $("#selected-team-contact-input").val("");
        $(".team--selected").removeClass("team--selected");
        event.preventDefault();
    });

    //If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#TeamModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
                $("#selected-team-name-input").val("");
                $("#selected-team-contact-input").val("");
                $(".team--selected").removeClass("team--selected");
            }
        }
    });

    //===================================================================================

    $(document).ready(function () {
        //Add theme modal
        $("#open-add-theme-modal-button").click(function () {
            $("#add-theme-submit-button").show();
            $("#edit-theme-submit-button").hide();
            $("#themeModal").show();
            $("#selected-theme-name-input").val("");
            $("#selected-theme-description-input").val("");

            event.preventDefault();
        });

        //Save edited theme
        $("#edit-theme-submit-button").click(function () {
            $(".theme--selected .theme-name-text").html($("#selected-theme-name-input").val());

            $(".theme--selected .theme-name-input").val($("#selected-theme-name-input").val());
            $(".theme--selected .theme-description-input").val($("#selected-theme-description-input").val());

            $("#selected-theme-name-input").val("");
            $("#selected-theme-description-input").val("");
            $(".theme--selected").removeClass("theme--selected");
            $("#themeModal").hide();
            event.preventDefault();
        });

        //On modal close
        $("#close-add-theme-modal-button, .themeModalClose").click(function () {
            $("#themeModal").hide();
            $("#selected-theme-name-input").val("");
            $("#selected-theme-description-input").val("");
            $(".theme--selected").removeClass("theme--selected");
            event.preventDefault();
        });

        //If user clicks outside the modal
        $(document).mouseup(function (e) {
            const modal = $("#ThemeModal");
            if (modal.is(":visible")) {
                // if the target of the click isn't the container nor a descendant of the container
                if (modal.is(e.target) && modal.has(e.target).length === 0) {
                    modal.hide();
                    $("#selected-theme-name-input").val("");
                    $("#selected-theme-description-input").val("");
                    $(".theme--selected").removeClass("theme--selected");
                }
            }
        });

    });
});
//============================================================================================

//Team
//Edit team modal
function openEditTeamModal($this) {
    event.preventDefault();
    $($this).closest(".team").addClass("team--selected");
    $("#add-team-submit-button").hide();
    $("#edit-team-submit-button").show();
    $("#selected-team-name-input").val($($this).closest(".team").find(".team-name-input").val());
    $("#selected-team-contact-input").val($($this).closest(".team").find(".team-contact-input").val());
    $("#teamModal").show();
    };

    //Theme
    //Edit theme modal
    function openEditThemeModal($this) {
        event.preventDefault();
        $($this).closest(".theme").addClass("theme--selected");
        $("#add-theme-submit-button").hide();
        $("#edit-theme-submit-button").show();
        $("#selected-theme-name-input").val($($this).closest(".theme").find(".theme-name-input").val());
        $("#selected-theme-description-input").val($($this).closest(".theme").find(".theme-description-input").val());
        $("#themeModal").show();
    };