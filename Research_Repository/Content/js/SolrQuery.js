//Solr query object
let queryParameters = {
    searchText = "",
    themes =[],
    teams =[],
    projects =[],
    tags =[],
    sensitivity =[],
    approvals =[],
    status=["Published"],
    userIds=[],
    startDate = "yyyy-mm-dd",
    endDate = "yyyy-mm-dd",
    paginationStartItem = 0
};

//Number of items per pagination page (also need to update in WC constants)
const numOfItemsPerPage = 10;

$(document).ready(function () {

    //Solr reindex
    $("#reindex-solr-button").click(function () {
        event.preventDefault();
        $.ajax({
            type: "GET",
            url: "/ItemRequest/ReindexItems",
            success: function (data) {

                if (data) {
                    alert("Reindex succeeded");
                } else {
                    alert("Reindex failed");
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    //On filter button click
    $("#filters .filter__button").click(function () {
        $(".filter__button").removeClass("button--active");
        $(this).addClass("button--active");
    })

    //On text change in text box
    $("#filters input:not(#text-search), #filters select").change(function () {
        queryParameters.paginationStartItem = 0;
        filterProjectsForItemsList();
    });

    //On date field value change
    $("#filters #text-search, #filters #start-date-search, #filters #end-date-search").keyup(function () {
        queryParameters.paginationStartItem = 0;
        filterProjectsForItemsList();
    });

    //If items library page is displayed, query solr for items with a 'Published' state
    if ($('#filters.published-filters').length) {
        filterProjectsForItemsList();
    }

    //If item requests page is displayed, query solr for items with a 'Submitted' state
    if ($('#filters.librarian-filters').length) {
        updateFilterParameters(queryParameters.paginationStartItem, "Submitted");
    }

    //If profile page is displayed, query solr for items with a 'Draft' state and created by the user
    if ($('#filters.profile-filters').length) {
        $("#drafts-filter-btn").addClass("button--active");
        //Get userId from session
        $.ajax({
            type: "GET",
            url: "/Profile/GetUserId",
            dataType: "text",
            success: function (data) {
                queryParameters.userId = [];
                queryParameters.userId.push(data);
            },
            error: function (error) {
                console.log(error);
            }
        });

        updateFilterParameters(queryParameters.paginationStartItem, "Draft");
    }

    //If the clear filter button is clicked
    $("#clear-filters").click(function () {
        clearFilters();
    })
});

//Update solr query object with selected filter options
function updateFilterParameters(pageId, itemStatus) {
    //Clear current filter options
    queryParameters.themes = [];
    queryParameters.teams = [];
    queryParameters.projects = [];
    queryParameters.tags = [];
    queryParameters.sensitivity = [];
    queryParameters.approvals = [];
    queryParameters.searchText = "";

    //Set search parameter
    queryParameters.searchText = $("#text-search").val();
    $("#approval-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.approvals.push($(this).data("name"));
    });
    //Set sensitivity parameter
    $("#sensitivity-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.sensitivity.push($(this).data("name"));
    });
    //Set tag parameter
    $("#tag-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.tags.push($(this).data("name"));
    });
    //Set team parameter
    $("#team-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.teams.push($(this).data("name"));
    });
    //Set theme parameter
    $("#theme-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.themes.push($(this).data("name"));
    });
    //Set project parameter
    $("#project-checkbox-filter label:visible input:checked").each(function (index, element) {
        queryParameters.projects.push($(this).data("name"));
    });

    //Set start date parameter
    if ($("#start-date-search").length) {
        queryParameters.startDate = $("#start-date-search").val();
    }
    //Set end date parameter
    if ($("#end-date-search").length) {
        queryParameters.endDate = $("#end-date-search").val();
    }

    //Get pagination start item
    queryParameters.paginationStartItem = (pageId * numOfItemsPerPage);

    //Set status parameter
    if (itemStatus) {
        //Clear status parameter
        queryParameters.status = [];
        //Add status parameter
        queryParameters.status.push(itemStatus);
    }

    //Filter items list
    filterItemList(itemStatus, pageId);
};

//Filter items list
function filterItemList(itemStatus, pageId) {
    //Stringify solr query parameter list
    let stringifiedParameters = JSON.stringify(queryParameters);
    $.ajax({
        type: "GET",
        url: "/Item/GetFilteredItems",
        data: { "itemQueryJson": stringifiedParameters },
        success: function (data) {
            let itemListHtml = "";

            if (!data || data.items.length == 0) {
                itemListHtml = `<div class="no-items"><p>No items to display<p></div>`;
            } else {

                for (let i = 0; i < data.items.length; i++) {

                    //Profile items
                    if ($('#filters.profile-filters').length) {
                        //Get card colour
                        let cardColour = "";
                        if (data.items[i].status == "Submitted") {
                            cardColour = "yellow";
                        } else if (data.items[i].status == "Published") {
                            cardColour = "green";
                        } else if (data.items[i].status == "Rejected") {
                            cardColour = "red";
                        } else {
                            cardColour = "blue";
                        }
                        //Get html for tags
                        let tagPillsHtml = "";
                        if (data.items[i].tags) {
                            for (let t = 0; t < data.items[i].tags.length; t++) {
                                tagPillsHtml += `
                <div class="pill">
                  <img
                    class="pill__image"
                    src="images/svgs/tag.svg"
                    alt="tag_image"
                  />${data.items[i].tags[t]}
                </div>`
                            }
                        }

                        itemListHtml +=
                            `<a href="/Item/View/${data.items[i].id}" class="card card--pathway ${cardColour !== "blue" ? `card--${cardColour}` : ""}">
${data.items[i].notifyUploader ? `<span class="notification-bubble notification-bubble__item-notification">!</span>` : ""}
            <div class="card__content">
              <div class="card__heading-container">
                <h4 class="card__heading">${data.items[i].title ? `${data.items[i].title}` : "No title"}</h4>
                <h4 class="card__subheading text--transparent-70-base-blue">
                  ${data.items[i].team ? `${data.items[i].team}` : ""}
                </h4>
              </div>

              <div class="card__description-container">
                <p class="text--black">
                  ${data.items[i].abstract}
                </p>
              </div>
                ${tagPillsHtml}
              </div>
            </div>
          </a>`;
                    }

                    //Librarian portal items
                    if ($('#filters.librarian-filters').length) {
                        itemListHtml +=
          `<a href="/Item/View/${data.items[i].id}" class="card card--pathway">
            <div class="card__content">
              <div class="card__heading-container">
                <h4 class="card__heading">${data.items[i].title ? `${data.items[i].title}` : "No title"}</h4>
              </div>

 <div class="margin-bottom--extra-small"><span class="text--black">Title: </span> <span class="faded">${data.items[i].title}</span></div>
 <div class="margin-bottom--extra-small"><span class="text--black">Contributer: </span> <span class="faded">${data.items[i].uploader}</span></div>
 <div class="margin-bottom--extra-small"><span class="text--black">Last updated: </span> <span class="faded">${data.items[i].lastUpdatedDate.split("T")[0]}</span></div>
            </div>
          </a>`;
                    }

                    //Library items
                    if ($('#filters.published-filters').length) {
                        //Get html for tags
                        let tagPillsHtml = "";
                        if (data.items[i].tags) {
                            for (let t = 0; t < data.items[i].tags.length; t++) {
                                tagPillsHtml += `
                <div class="pill">
                  <img
                    class="pill__image"
                    src="images/svgs/tag.svg"
                    alt="tag_image"
                  />${data.items[i].tags[t]}
                </div>`
                            }
                        }

                        itemListHtml +=
                            `<a href="/Item/View/${data.items[i].id}" class="card card--pathway">
            <div class="card__content">
              <div class="card__heading-container">
                 <h4 class="card__heading">${data.items[i].title ? `${data.items[i].title}` : "No title"}</h4>
                <h4 class="card__subheading text--transparent-70-base-blue">
                  ${data.items[i].team ? `${data.items[i].team}` : ""}
                </h4>
              </div>

              <div class="card__description-container">
                <p class="text--black">
                  ${data.items[i].abstract}
                </p>
              </div>
                ${tagPillsHtml}
              </div>
            </div>
          </a>`;
                    }
                };
            }
            //Add items to HTML
            $("#item-list").html(itemListHtml);

            //Pagination
            let itemListPaginationHtml = "";
            let numOfTotalPages = 0;
            let numOfMiddlePages = 0;
            if (data.numOfTotalResults <= numOfItemsPerPage) {
                numOfTotalPages = 0;
                numOfMiddlePages = 0;
            } else {
                numOfTotalPages = Math.ceil(data.numOfTotalResults / numOfItemsPerPage);
                numOfMiddlePages = numOfTotalPages - 2;
            }
            if (numOfTotalPages < 2) {
                return
            }
            pageNum = pageId + 1;

            //Previous and last pagination buttons
            //If selected pageId is not the first pageId
            if (pageNum >= 2) {
                itemListPaginationHtml +=
                    `<li class="pagination__item">
                     <button class="pagination__button" onclick="updateFilterParameters(${pageId - 1}, '${itemStatus}')">Previous</button>
                     </li>`;
            } else {
                itemListPaginationHtml +=
                    `<li class="pagination__item">
                    <button class="pagination__button" disabled>Previous</button>
                    </li>`;
            }
            //First pageId
            itemListPaginationHtml +=
                `<li class="pagination__item">
                <button class="pagination__button ${pageNum == 1 ? "pagination__button--active" : ""}" onclick="updateFilterParameters(${0}, '${itemStatus}')">1</button>
                </li>`;

            //...
            if (pageNum >= 4) {
                itemListPaginationHtml += `<li class="pagination__item">...</li>`;
            }

            //If number of pageIds is 3 or more
            if (numOfMiddlePages >= 1) {
                //If selected button is not pageNum 1 or 2
                if (pageNum <= 2) {
                    for (let i = pageId; i <= 3; i++) {
                        if (i > 0) {
                            itemListPaginationHtml +=
                            `<li class="pagination__item">
                            <button class="pagination__button ${pageNum == i + 1 ? "pagination__button--active" : ""}"  onclick="updateFilterParameters(${i}, '${itemStatus}')">
                            ${i + 1}
                            </button>
                            </li>`;
                        }
                    };
                    //If selected button is not the final 2 pageIds
                } else if (pageNum >= numOfTotalPages - 1) {
                    for (let i = numOfMiddlePages - 2; i <= numOfMiddlePages; i++) {
                        itemListPaginationHtml +=
                           `<li class="pagination__item">
                            <button class="pagination__button ${pageNum == i + 1 ? "pagination__button--active" : ""}" onclick="updateFilterParameters(${i}, '${itemStatus}')">
                            ${i + 1}
                            </button>
                            </li>`;
                    }
                } else {
                    for (let i = pageId - 1; i <= pageId + 1; i++) {
                        itemListPaginationHtml +=
                            `<li class="pagination__item">
                            <button class="pagination__button ${pageNum == i + 1 ? "pagination__button--active" : ""}" onclick="updateFilterParameters(${i}, '${itemStatus}')">
                            ${i + 1}
                            </button>
                            </li>`;
                    };
                }
            }

            //...
            if (pageNum <= numOfTotalPages - 3) {
                itemListPaginationHtml += `<li class="pagination__item">...</li>`;
            }

            if (numOfTotalPages >= 1) {
                //Last page
                itemListPaginationHtml +=
                    `<li class="pagination__item">
                    <button class="pagination__button ${pageNum == numOfTotalPages ? "pagination__button--active" : ""}" onclick="updateFilterParameters(${numOfTotalPages - 1}, '${itemStatus}')">
                    ${numOfTotalPages}
                    </button>
                    </li >`;
            }
            //Next page
            if (pageNum < numOfTotalPages) {
                itemListPaginationHtml +=
                    `<li class="pagination__item">
                    <button class="pagination__button" onclick="updateFilterParameters(${pageId + 1}, '${itemStatus}')">Next</button>
                     </li>`;
            } else {
                itemListPaginationHtml +=
              `<li class="pagination__item">
              <button class="pagination__button" disabled>Next</button>
              </li>`;
            }

            $("#item-list-pagination").html(itemListPaginationHtml);
        },
        error: function (error) {
            console.log(error);
        }
    })
};

//Filter tags checklist based on theme selected
function filterTagsForItemsList() {
    let themeIds = [];
    //Get theme ids of all checked theme checkboxes
    $("#theme-checkbox-filter input:checked").each(function (index, element) {
        //Check all relevant label checkboxes
        if (!$(this).parent().hasClass("field__label--checkbox-checked")) {
            $(this).parent().addClass("field__label--checkbox-checked");
        }

        //Expand relevant accordion if it's not open
        if (!$(this).closest(".accordion__container").find(".accordion").first().hasClass("accordion--active")) {
            $(this).closest(".accordion__container").find(".accordion").first().addClass("accordion--active");
            $(this).closest(".accordion__container").find(".accordion").first().next(".accordion__content").slideDown();
        }

        themeIds.push($(this).parent().next("input").val());
    });

    $.ajax({
        type: "GET",
        url: "/Item/GetThemeTags",
        data: { "ids": themeIds },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (themeIds !== undefined && themeIds.length > 0) {
                $("#tag-checkbox-filter label").addClass("hidden");
                for (var i = 0; i < data.length; i++) {
                    $("#tag-input-id-" + data[i]).parent().removeClass("hidden");
                }
            } else {
                $("#tag-checkbox-filter label").removeClass("hidden");
            }
            updateFilterParameters(queryParameters.paginationStartItem, "Published");
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Filter projects select list based on team selected
function filterProjectsForItemsList() {
    let teamIds = [];
    //Get team ids of all checked team checkboxes
    $("#team-checkbox-filter input:checked").each(function (index, element) {
        teamIds.push($(this).parent().next("input").val());
    });

    $("#project-checkbox-filter input:checked").each(function (index, element) {
        //Check all relevant label checkboxes
        if (!$(this).parent().hasClass("field__label--checkbox-checked")) {
            $(this).parent().addClass("field__label--checkbox-checked");
        }

        //Expand relevant accordion if it's not open
        if (!$(this).closest(".accordion__container").find(".accordion").first().hasClass("accordion--active")) {
            $(this).closest(".accordion__container").find(".accordion").first().addClass("accordion--active");
            $(this).closest(".accordion__container").find(".accordion").first().next(".accordion__content").slideDown();
        }
    });

    $.ajax({
        type: "GET",
        url: "/Item/GetTeamProjects",
        data: { "ids": teamIds },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (data) {
            if (teamIds !== undefined && teamIds.length > 0) {
                $("#project-checkbox-filter label").addClass("hidden");
                for (var i = 0; i < data.length; i++) {
                    $("#project-input-id-" + data[i]).parent().removeClass("hidden");
                }
            } else {
                $("#project-checkbox-filter label").removeClass("hidden");
            }
            //Filter tag list from theme selection
            filterTagsForItemsList();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Clear filter list
function clearFilters() {
    queryParameters.searchText = $("#text-search").val("");
    $("#filters input:checked").prop('checked', false);
    $(".field__label--checkbox").removeClass("field__label--checkbox-checked");

    $("#filters label").removeClass("hidden");

    $("#start-date-search").val("");

    $("#end-date-search").val("");

    updateFilterParameters(0, "Published");
}