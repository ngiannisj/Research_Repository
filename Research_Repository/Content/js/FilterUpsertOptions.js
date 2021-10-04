$(document).ready(function () {

    //Set checked state after checkbox click
    $(":checkbox").change(function () {
        if ($(this).is(':checked')) {
            $(this).attr('checked', true);
            $(this).parent().addClass("field__label--checkbox-checked");
        } else {
            $(this).attr('checked', false);
            $(this).parent().removeClass("field__label--checkbox-checked");
        }
    });

    //Filter tags on page load
    if ($("#filters").length) {
        filterTags($("#theme-selector"));
    }

    //Filter projects on page load
    if ($("#filters").length) {
        filterProjects($("#team-selector"));
    }

    //Filter tags on theme change
    $("#theme-selector").change(function () {
        filterTags($(this));
    });

    //Filter projects on team change
    $("#team-selector").change(function () {
        filterProjects($(this));
    });

});

//Filter tags checklist based on theme selected
function filterTags($this) {

    let selectedThemeIds = [parseInt($this.find(":selected").attr("value"))];

    if (!selectedThemeIds) {
        return
    };

    $.ajax({
        type: "GET",
        url: "/Item/GetThemeTags",
        data: { "ids": selectedThemeIds },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (data) {
                $("#checkboxes label").hide();
                for (var i = 0; i < data.length; i++) {
                    $("#tag-input-id-" + data[i]).parent().show();
                }
            }
        },
        error: function () {
            console.log(error);
        }
    });
}

//Filter projects select list based on team selected
function filterProjects($this) {

    let selectedTeamIds = [parseInt($this.find(":selected").attr("value"))];

    if (!selectedTeamIds) {
        return
    };

    $.ajax({
        type: "GET",
        url: "/Item/GetTeamProjects",
        data: { "ids": selectedTeamIds },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (data) {
                $("#project-selector option:not(:first)").hide();
                for (var i = 0; i < data.length; i++) {
                    $("#project-selector option[value=" + data[i] + "]").show();
                }
            }

        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Create checkbox dropdown list
var expanded = false;
function showCheckboxes() {
    if ($("#tag-selector select").prop('disabled') == false) {
        var checkboxes = document.getElementById("checkboxes");
        if (!expanded) {
            checkboxes.style.display = "block";
            expanded = true;
        } else {
            checkboxes.style.display = "none";
            expanded = false;
        }
    }
}