$(document).ready(function () {
    //Opens items search page with filters pre-selected based on button clicked
    $(".homepage-filter").click(function () {
        const type = $(this).data("type");
        const name = $(this).data("name");

        window.location.href = `../item?filterType=${type}&checkedCheckbox=${name}`;
    });

    //Opens items search page filtered based on text input into serch bar
    $("#homepage-submit-btn").click(function () {
        const searchText = $("#homepage-searchbar").val();
        window.location.href = `../item?searchText=${searchText}`;
    });
});