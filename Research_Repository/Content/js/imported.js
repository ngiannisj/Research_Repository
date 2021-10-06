$(document).ready(function () {

    //Enable focus clicking for divs with certain roles
    document.querySelectorAll('[role="button"], [role="tab"], [role="link"]').forEach(function (n) {
        n.addEventListener("keydown", function (t) {
            if (t.keyCode == 32) {
                event.preventDefault(); //Stops spacebar from scrolling page
            }
            (t.keyCode == 13 || t.keyCode == 32) && n.click()

        })
    })
    //======================================================================================================
    //Accordion
    //Stops accordion from opening if a button inside the accordion button is clicked
    $(".accordion__button .button--icon").click(function () {
        event.stopPropagation();
    });

    $(".accordion__button .button--icon").keydown(function () {
        event.stopPropagation();
    });

    $(".accordion__button .button--icon").mouseover(function () {
        $(this).closest(".accordion").addClass("no-hover");
    });

    $(".accordion__button .button--icon").mouseleave(function () {
        $(this).closest(".accordion").removeClass("no-hover");
    });

    //Basic accordion functionality
    $(".accordion").click(function () {
        $(this).toggleClass("accordion--active");
        if ($(this).hasClass("accordion--active")) {
            $(this).next(".accordion__content").slideDown();
        } else {
            $(this).next(".accordion__content").slideUp();
        }
    });

    // Close the accordion if the user clicks outside of selectlist
    $(window).click(function (event) {
        if (!event.target.matches(".accordion__button--select-list")) {
            const selectListBtns = $(".accordion__button--select-list");
            let i;
            for (i = 0; i < selectListBtns.length; i++) {
                $(selectListBtns[i]).removeClass("accordion--active");
                $(selectListBtns[i]).next(".accordion__content").slideUp();
            }
        }
    });

    //Accordion focus on accordion button parent container
    $(".accordion__button").focus(function () {
        $(this).closest(".accordion__container").addClass("focus");
    });

    $(".accordion__button").blur(function () {
        $(this).closest(".accordion__container").removeClass("focus");
    });

    //===================================
    //Back button to go back to previous page
    $(".link--back").click(function () {
        window.history.back();
    })
});

//==================================================================

//Selectlist accordion functionality
function selectListOptionClick($this) {
    $($this)
        .closest(".accordion__container--select-list")
        .find(".accordion__button--select-list")
        .first()
        .attr("data-selected", $($this).data("value"));
    $($this)
        .closest(".accordion__container--select-list")
        .find(".accordion__button--select-list")
        .first()
        .html($($this).text());

    $($this).closest(".accordion__container").find(".select-list-input").first().val($($this).data("value"));
    event.preventDefault();
}

function selectListButtonClick() {
    event.preventDefault();
}


//==================================================================
//Navigation item selected
$(document).ready(function () {
    highlightSelectedHeaderNavLink();
    highlightSelectedSidebarNavLink();
});

function highlightSelectedHeaderNavLink() {
    if (window.location.href.toLowerCase().includes("item/upsert")) {
        $("#contribute-nav-link").addClass("header__nav-link--active");
        return
    } else if (window.location.href.toLowerCase().includes("item") && !window.location.href.toLowerCase().includes("itemrequest") && !window.location.href.toLowerCase().includes("account") || window.location.href.toLowerCase().includes("item?filterType")) {
        $("#library-nav-link").addClass("header__nav-link--active");
        return
    }
    else if (window.location.href.toLowerCase().includes("itemrequest") || window.location.href.toLowerCase().includes("theme") || window.location.href.toLowerCase().includes("team") || window.location.href.toLowerCase().includes("account")) {
        $("#librarian-nav-link").addClass("header__nav-link--active");
        $("#login-nav-link").addClass("header__nav-link--active");
        return
    }
    else if (window.location.href.toLowerCase().includes("profile")) {
        $("#profile-nav-link").addClass("header__nav-link--active");
        return
    }
    else {
        $("#home-nav-link").addClass("header__nav-link--active");
    }
};

function highlightSelectedSidebarNavLink() {
    if (window.location.href.toLowerCase().includes("itemrequest")) {
        $("#item-request-nav-link").addClass("sidebar-nav__nav-link--active");
        return
    } else if (window.location.href.toLowerCase().includes("theme")) {
        $("#theme-nav-link").addClass("sidebar-nav__nav-link--active");
        return
    }
    else if (window.location.href.toLowerCase().includes("team")) {
        $("#team-nav-link").addClass("sidebar-nav__nav-link--active");
        return
    }
    else if (window.location.href.toLowerCase().includes("account")) {
        $("#create-librarian-nav-link").addClass("sidebar-nav__nav-link--active");
        return
    }
};

//==============================================================================
//Lock modal focus
function findInsiders(elem) {
    let tabbable = elem
        .find("select, input, textarea, button, a")
        .filter(":visible");

    let firstTabbable = tabbable.first();
    let lastTabbable = tabbable.last();

    /*set focus on first input*/
    firstTabbable.focus();

    /*redirect last tab to first input*/
    lastTabbable.on("keydown", function (e) {
        if (e.which === 9 && !e.shiftKey) {
            e.preventDefault();
            firstTabbable.focus();
        }
    });

    /*redirect first shift+tab to last input*/
    firstTabbable.on("keydown", function (e) {
        if (e.which === 9 && e.shiftKey) {
            e.preventDefault();
            lastTabbable.focus();
        }
    });

    /* allow escape key to close insiders div */
    elem.on("keyup", function (e) {
        if (e.keyCode === 27) {
            elem.hide();
        }
    });
};