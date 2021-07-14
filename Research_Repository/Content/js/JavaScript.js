$(document).ready(function () {
    let selectedThemeId = "";
    $("#theme-selector").change(function () {
        selectedThemeId = parseInt($(this).find(":selected").attr("value"));
        console.log(selectedThemeId);
        $.ajax({
            type: "GET",
            url: "/Item/GetAssignedTags",
            data: { "id": selectedThemeId },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                //hide non-applicable checkboxes
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