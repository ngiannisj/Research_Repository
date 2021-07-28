$(document).ready(function () {
    const queryParameters = JSON.stringify({
        searchText = "*",
        teams = ["*"], //Create array
        projects =["*"], //Create array
        tags = ["item 10"], //Create array
        startDate = "*",
        endDate = "*"
    });

    $("#filters input:not(#text-search), #filters select").change(function () {
        alert("Handler for .change() called.");
    });
    $("#filters #text-search").keypress(function () {
        $.ajax({
            type: "POST",
            url: "/Item/PostFilteredItems",
            data: { "itemQueryJson": queryParameters },
            success: function (data) {
                console.log(data);
            },
            error: function () {
                alert("Error occured!!")
            }
    });

    });
});