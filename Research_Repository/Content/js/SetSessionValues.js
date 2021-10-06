$(document).ready(function () {
    //On page load set notification value for new items in header
    if ($("#profile-nav-link").length) {
        setNotificationValue();
    }

    //If 'Item request' page nav link is showing on screen, update number of item request value
    if ($("#item-request-count").length) {
        setItemRequestValue();
    }
});

//Update notification value for updated item statuses in session
function setNotificationValue() {
    $.ajax({
        url: "/Item/AddNotificationsToSession",
        type: "GET",
        dataType: 'json',
        success: function (data) {
            if (data) {
                let newDraftCount = [];
                let newPublishedCount = [];
                let newRejectedCount = [];

                data.forEach(function (status, index) {
                    if (status == "Draft") {
                        newDraftCount.push(status);
                    } else if (status == "Published") {
                        newPublishedCount.push(status);
                    } else if (status == "Rejected") {
                        newRejectedCount.push(status);
                    }
                });

                if (newDraftCount.length) {
                    $("#profile-draft-notification").html("!");
                    $("#profile-draft-notification").removeClass("notification-bubble--hidden");
                }
                if (newPublishedCount.length) {
                    $("#profile-published-notification").html("!");
                    $("#profile-published-notification").removeClass("notification-bubble--hidden");
                }
                if (newRejectedCount.length) {
                    $("#profile-rejected-notification").html("!");
                    $("#profile-rejected-notification").removeClass("notification-bubble--hidden");
                }
                if (data.length) {
                    $("#item-notification-count").html(data.length);
                    $("#profile-nav-link").addClass("header__nav-link--has-notification");
                    $("#item-notification-count").removeClass("notification-bubble--hidden");
                }
            }
        },
        error: function (request, error) {

        }
    });
};

//Update notification value for items in 'Submitted' state for librarians in session
function setItemRequestValue() {
    $.ajax({
        url: "/Item/AddItemRequestCountToSession",
        type: "GET",
        dataType: 'json',
        success: function (data) {
            if (data) {
                $("#item-request-count").html(data);
                $("#item-request-count").removeClass("notification-bubble--hidden");
            }
        },
        error: function (request, error) {

        }
    });
};
