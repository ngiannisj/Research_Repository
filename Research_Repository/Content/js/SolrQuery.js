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
    startDate = "0001-01-01",
    endDate = "0001-01-01",
    paginationStartItem = 0
};

const numOfItemsPerPage = 1;

$(document).ready(function () {

    $("#filters input:not(#text-search), #filters select").change(function () {
        queryParameters.paginationStartItem = 0;
        filterProjectsForItemsList();
    });

    $("#filters #text-search, #filters #start-date-search, #filters #end-date-search").keyup(function () {
        queryParameters.paginationStartItem = 0;
        filterProjectsForItemsList();
    });

    if ($('#filters.published-filters').length) {
        updateFilterParameters(queryParameters.paginationStartItem, "Published");
    }

    if ($('#filters.librarian-filters').length) {
        updateFilterParameters(queryParameters.paginationStartItem, "Submitted");
    }

    if ($('#filters.profile-filters').length) {
        //Get userId
        $.ajax({
            type: "GET",
            url: "/Profile/GetUserId",
            dataType: "text",
            success: function (data) {
                queryParameters.userId = [];
                queryParameters.userId.push(data);
            },
            error: function () {
                alert("Error occured!!")
            }
        });

        updateFilterParameters(queryParameters.paginationStartItem, "Draft");
    }

    $("#clear-filters").click(function () {
        clearFilters();
    })
});


function updateFilterParameters(pageId, itemStatus) {
    queryParameters.themes = [];
    queryParameters.teams = [];
    queryParameters.projects = [];
    queryParameters.tags = [];
    queryParameters.sensitivity = [];
    queryParameters.approvals = [];
    queryParameters.searchText = "";

    queryParameters.searchText = $("#text-search").val();
    $("#approval-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.approvals.push($(this).data("name"));
    });
    $("#sensitivity-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.sensitivity.push($(this).data("name"));
    });
    $("#tag-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.tags.push($(this).data("name"));
    });
    $("#team-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.teams.push($(this).data("name"));
    });
    $("#theme-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.themes.push($(this).data("name"));
    });
    $("#project-checkbox-filter input:visible:checked").each(function (index, element) {
        queryParameters.projects.push($(this).data("name"));
    });

    if ($("#start-date-search").length) {
        queryParameters.startDate = $("#start-date-search").val();
    }

    if ($("#end-date-search").length) {
        queryParameters.endDate = $("#end-date-search").val();
    } else {
        let date = new Date();
        const offset = date.getTimezoneOffset();
        date = new Date(date.getTime() - (offset * 60 * 1000));
        queryParameters.endDate = date.toISOString().split('T')[0];
    }

    queryParameters.paginationStartItem = (pageId * numOfItemsPerPage);

    if (itemStatus) {
        queryParameters.status = [];
        queryParameters.status.push(itemStatus);
    }

    filterItemList(itemStatus, pageId);
};

function filterItemList(itemStatus, pageId) {
    let stringifiedParameters = JSON.stringify(queryParameters);
    $.ajax({
        type: "GET",
        url: "/Item/GetFilteredItems",
        data: { "itemQueryJson": stringifiedParameters },
        success: function (data) {
            let itemListHtml = "";
            for (let i = 0; i < data.items.length; i++) {

                //Profile items
                if ($('#filters.profile-filters').length) {
                    itemListHtml += `<tr>
                    <td width="33%">${data.items[i].title}</td>
                    <td width="33%">${data.items[i].team}</td>
                    <td width="33%">${data.items[i].abstract}</td>
                    <td class="text-center">
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-primary mx-2" href="/Item/Upsert/${data.items[i].id}">
                                View${(itemStatus != "Submitted" && data.items[i].notifyUploader) ? "!" : ""}
                            </a>
                            ${itemStatus == "Draft" ?
                        `<button class="btn btn-danger" id="open-delete-item-modal-btn" value="${data.items[i].id}" onclick="showItemDeleteModal(this)">
                            Delete
                         </button>` : ""}
                        </div>
                    </td>
                </tr>`;
                }

                //Librarian portal items
                if ($('#filters.librarian-filters').length) {
                    itemListHtml += `<tr>
                    <td width="33%">${data.items[i].title}</td>
                    <td width="33%">${data.items[i].team}</td>
                    <td width="33%">${data.items[i].abstract}</td>
                    <td class="text-center">
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-primary mx-2" href="/Item/Upsert/${data.items[i].id}">
                                View
                            </a>
                        </div>
                    </td>
                </tr>`;
                }

                //Library items
                if ($('#filters.published-filters').length) {
                    itemListHtml += `<tr>
                    <td width="33%">${data.items[i].title}</td>
                    <td width="33%">${data.items[i].team}</td>
                    <td width="33%">${data.items[i].abstract}</td>
                    <td class="text-center">
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-primary mx-2" href="/Item/View/${data.items[i].id}">
                                View
                            </a>
                        </div>
                    </td>
                </tr>`;
                }


            };
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
            pageNum = pageId + 1;

            //Previous and last pagination buttons
            //If selected pageId is not the first pageId
            if (pageNum >= 2) {
                itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${pageId - 1}, '${itemStatus}')">&#60;Prev</button>`;
            } else {
                itemListPaginationHtml += `<button class="pagination-item" disabled>&#60;Prev</button>`;
            }
            //First pageId
            itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${0}, '${itemStatus}')">1</button>`;

            //...
            if (pageNum >= 4) {
                itemListPaginationHtml += `<button class="pagination-item">...</button>`;
            }

            //If number of pageIds is 3 or more
            if (numOfMiddlePages >= 1) {
                //If selected button is not pageNum 1 or 2
                if (pageNum <= 2) {
                    for (let i = pageId; i <= 3; i++) {
                        if (i > 0) {
                            itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${i}, '${itemStatus}')">${i + 1}</button>`;
                        }
                    };
                    //If selected button is not the final 2 pageIds
                } else if (pageNum >= numOfTotalPages - 1) {
                    for (let i = numOfMiddlePages - 2; i <= numOfMiddlePages; i++) {
                            itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${i}, '${itemStatus}')">${i + 1}</button>`;
                    }
                } else {
                    for (let i = pageId - 1; i <= pageId + 1; i++) {
                            itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${i}, '${itemStatus}')">${i + 1}</button>`;
                    };
                }
            }

            //...
            if (pageNum <= numOfTotalPages - 3) {
                itemListPaginationHtml += `<button class="pagination-item">...</button>`;
            }

            if (numOfTotalPages >= 1) {
                //Last page
                itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${numOfTotalPages - 1}, '${itemStatus}')">${numOfTotalPages}</button>`;
            }
            //Next page
            if (pageNum < numOfTotalPages) {
                itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${pageId + 1}, '${itemStatus}')">Next&#62;</button>`;
            } else {
                itemListPaginationHtml += `<button class="pagination-item" disabled>Next&#62;</button>`;
            }

            $("#item-list-pagination").html(itemListPaginationHtml);
        },
        error: function () {
            alert("Error occured!!")
        }
    })
};

//Filter tags checklist based on theme selected
function filterTagsForItemsList() {
    let themeIds = [];
    $("#theme-checkbox-filter input:checked:visible").each(function (index, element) {
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
                $("#tag-checkbox-filter label").hide();
                for (var i = 0; i < data.length; i++) {
                    $("#tag-input-id-" + data[i]).parent().show();
                }
            } else {
                $("#tag-checkbox-filter label").show();
            }
            updateFilterParameters(queryParameters.paginationStartItem, "Published");
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

//Filter projects select list based on team selected
function filterProjectsForItemsList() {
    let teamIds = [];
    $("#team-checkbox-filter input:checked:visible").each(function (index, element) {
        teamIds.push($(this).parent().next("input").val());
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
                $("#project-checkbox-filter label").hide();
                for (var i = 0; i < data.length; i++) {
                    $("#project-input-id-" + data[i]).parent().show();
                }
            } else {
                $("#project-checkbox-filter label").show();
            }
            //Filter tag list from theme selection
            filterTagsForItemsList();
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function clearFilters() {
    queryParameters.searchText = $("#text-search").val("");
    $("#approval-checkbox-filter input:checked").prop('checked', false);
    $("#sensitivity-checkbox-filter input:checked").prop('checked', false);
    $("#tag-checkbox-filter input:checked").prop('checked', false);
    $("#team-checkbox-filter input:checked").prop('checked', false);
    $("#theme-checkbox-filter input:checked").prop('checked', false);
    $("#project-checkbox-filter input:checked").prop('checked', false);

    $("#filters label").show();

        $("#start-date-search").val("0001-01-01");

    //Set end date as todays date
        let date = new Date();
        const offset = date.getTimezoneOffset();
        date = new Date(date.getTime() - (offset * 60 * 1000));
    $("#end-date-search").val(date.toISOString().split('T')[0]);

    updateFilterParameters(0, "Published");
}