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
    paginationStartItem = "0"
};

const numOfItemsPerPage = 2;

$(document).ready(function () {

    $("#filters input:not(#text-search), #filters select").change(function () {
        //Filter project list from team selection
        filterProjectsForItemsList();
    });

    $("#filters #text-search, #filters #start-date-search, #filters #end-date-search").keyup(function () {
        queryParameters.searchText = $("#text-search").val();
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
                console.log(data);
            },
            error: function () {
                alert("Error occured!!")
            }
        });

        updateFilterParameters(queryParameters.paginationStartItem, "Draft");
    }
});


function updateFilterParameters(page, itemStatus) {
    queryParameters.themes = [];
    queryParameters.teams = [];
    queryParameters.projects = [];
    queryParameters.tags = [];
    queryParameters.sensitivity = [];
    queryParameters.approvals = [];
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

    queryParameters.paginationStartItem = (page * numOfItemsPerPage);

    if (itemStatus) {
        queryParameters.status = [];
        queryParameters.status.push(itemStatus);
    }

    filterItemList(itemStatus);
};

function filterItemList(itemStatus) {
    let stringifiedParameters = JSON.stringify(queryParameters);
    $.ajax({
        type: "GET",
        url: "/Item/GetFilteredItems",
        data: { "itemQueryJson": stringifiedParameters },
        success: function (data) {
            let itemListHtml = "";
            for (let i = 0; i < data.items.length; i++) {

                //Profile items
                if (itemStatus == "Draft") {
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

                //Librarian portal items
                if (itemStatus == "Submitted") {
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
                if (itemStatus == "Published" || itemStatus == "Rejected") {
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
            console.log(queryParameters);
            console.log(data);
            $("#item-list").html(itemListHtml);

            //Pagination
            let itemListPaginationHtml = "";
            let numOfPages = 0;
            if (data.numOfTotalResults <= numOfItemsPerPage) {
                numOfPages = 0;
            } else {
                numOfPages = Math.ceil(data.numOfTotalResults / numOfItemsPerPage) - 1;
            }

            for (let i = 0; i <= numOfPages; i++) {
                itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${i}, '${itemStatus}')">${i + 1}</button>`;
                $("#item-list-pagination").html(itemListPaginationHtml);
            };
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