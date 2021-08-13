//Verify no linked children will be affected by parent delete
function verifyDelete(controller, id) {

    if (!id) {
        console.log("wrong");
    };

    $.ajax({
        type: "GET",
        url: "/" + controller + "/Delete",
        data: { "id": id },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data == 3) {
                alert("Items exist under this theme, move them before deleting");
            } else if (data == 2) {
                alert("Deleted successfully");
                location.reload();
            } else {
                
            }
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}