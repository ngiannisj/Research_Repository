$(document).ready(function () {
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