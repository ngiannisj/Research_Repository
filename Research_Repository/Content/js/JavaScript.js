$(document).ready(function () {

    //Filter tags checklist based on theme selected
    let selectedThemeId = "";
    $("#theme-selector").change(function () {
        selectedThemeId = parseInt($(this).find(":selected").attr("value"));
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

    })

    //Filter projects dropdown based on team selected
    let selectedTeamId = "";
    $("#team-selector").change(function () {
        selectedTeamId = parseInt($(this).find(":selected").attr("value"));
        $.ajax({
            type: "GET",
            url: "/Item/GetTeamProjects",
            data: { "id": selectedTeamId },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                console.log("thing")
                console.log(data);
            },
            error: function () {
                alert("Error occured!!")
            }
        });

    })
 
});

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