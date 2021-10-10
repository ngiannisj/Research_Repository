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

    $(":radio").change(function () {
        $(this).closest(".field__list-container").find(".field__label--radio-checked").removeClass("field__label--radio-checked");
        $(this).closest(".field__list-container").find(".field__input-radio").attr('checked', false);

        if ($(this).is(':checked')) {
            $(this).attr('checked', true);
            $(this).parent().addClass("field__label--radio-checked");
        }
    });


    //Filter tags on page load
    if ($("#item-form").length) {
        filterTags($("#theme-selector").val());
    }

    //Filter projects on page load
    if ($("#item-form").length) {
        filterProjects($("#team-selector").val());
    }

    //Filter tags on theme change
    $("#item-theme-selectList-container .accordion__content-option").click(function () {
        filterTags($(this).data("value"));
    });

    //Filter projects on team change
    $("#item-team-selectList-container .accordion__content-option").click(function () {
        filterProjects($(this).data("value"));
    });

});

//Filter tags checklist based on theme selected
function filterTags(id) {
    if (!id) {
        return
    }
    $.ajax({
        type: "GET",
        url: "/Item/GetThemeTags",
        data: { "ids": id },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (data) {
                $(".field__list-item--checkbox").addClass("hidden");
                $("#tag-selector label").addClass("disabled");
                $("#tag-selector .field__input-checkbox").prop('disabled', true);
            for (var i = 0; i < data.length; i++) {
                $("#tag-input-id-" + data[i]).closest(".field__list-item").removeClass("hidden");
                $("#tag-input-id-" + data[i]).prop('disabled', false);
                $("#tag-input-id-" + data[i]).parent().removeClass("disabled");
            }
        } else {
                $("#tag-checkbox-filter").closest(".field__list-item").removeClass("hidden");
                $("#tag-checkbox-filter label").removeClass("disabled");
                $("#tag-selector .field__input-checkbox").prop('disabled', false);
        }
        },
        error: function () {
            console.log(error);
        }
    });
}

//Filter projects select list based on team selected
function filterProjects(id) {
    if (!id) {
        return
    }
    $.ajax({
        type: "GET",
        url: "/Item/GetTeamProjects",
        data: { "ids": id },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (data) {
                $("#project-selector-container .accordion__content-option").addClass("hidden");
                for (var i = 0; i < data.length; i++) {
                    $("#project-button-" + data[i]).removeClass("hidden");
                }
            } else {
                $("#project-selector-container .accordion__content-option").removeClass("hidden");
            }

        },
        error: function (error) {
            console.log(error);
        }
    });
}