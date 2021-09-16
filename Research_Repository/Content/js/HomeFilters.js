$(document).ready(function () {
    $(".homepage-filter").click(function () {
        const type = $(this).data("type");
        const name = $(this).data("name");

        window.location.href = `../item?filterType=${type}&checkedCheckbox=${name}`;
    });

    $("#homepage-submit-btn").click(function () {
        const searchText = $("#homepage-searchbar").val();
        window.location.href = `../item?searchText=${searchText}`;
    });
});