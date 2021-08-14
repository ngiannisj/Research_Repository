$(document).ready(function () {


        $(":checkbox").change(function () {
            if ($(this).is(':checked')) {
                $(this).attr('checked', true);
            } else {
                $(this).attr('checked', false);
            }
        })

    //Filter tags on page load
    filterTags($("#theme-selector"));

    //Filter projects on page load
    filterProjects($("#team-selector"));

    $("#theme-selector").change(function () {
        filterTags($(this));
    });

    $("#team-selector").change(function () {
        filterProjects($(this));
    });
 
});

//Filter tags checklist based on theme selected
function filterTags($this) {

    let selectedThemeId = parseInt($this.find(":selected").attr("value"));

    if (!selectedThemeId) {
        return
    };

    $.ajax({
        type: "GET",
        url: "/Item/GetThemeTags",
        data: { "id": selectedThemeId },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#checkboxes label").hide();
            for (var i = 0; i < data.length; i++) {
                $("#tag-input-id-" + data[i]).parent().show();
            }
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

//Filter projects select list based on team selected
function filterProjects($this) {

    let selectedTeamId = parseInt($this.find(":selected").attr("value"));

    if (!selectedTeamId) {
        return
    };

    $.ajax({
        type: "GET",
        url: "/Item/GetTeamProjects",
        data: { "id": selectedTeamId },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#project-selector option").hide();
            for (var i = 0; i < data.length; i++) {
                $("#project-selector option[value=" + data[i] + "]").show();
            }
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

//Create checkbox dropdown
var expanded = false;
function showCheckboxes() {
    var checkboxes = document.getElementById("checkboxes");
    if (!expanded) {
        checkboxes.style.display = "block";
        expanded = true;
    } else {
        checkboxes.style.display = "none";
        expanded = false;
    }
}