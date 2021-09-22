$(document).ready(function () {
    //On page load set notification value for new items in header
    setNotificationValue();

    //If 'Item request' page nav link is showing on screen, update number of item request value
    if ($(".item-request-link").length) {
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
            }
            if (newPublishedCount.length) {
                $("#profile-published-notification").html("!");
            }
            if (newRejectedCount.length) {
                $("#profile-rejected-notification").html("!");
            }
            if (data.length) {
                $("#item-notification-count").html("!");
            }
        },
        error: function (request, error) {
            alert("Request: " + JSON.stringify(request));
            console.log(error);
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
            $("#item-request-count").html(data);
        },
        error: function (request, error) {
            alert("Request: " + JSON.stringify(request));
            console.log(error);
        }
    });
};
