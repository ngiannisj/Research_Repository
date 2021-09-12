let queryParameters = {
    searchText = "",
    themes =[],
    teams =[],
    projects =[],
    tags =[],
    sensitivity =[],
    approvals =[],
    startDate = "00-00-0001",
    endDate = "00-00-0001",
    paginationStartItem = "0"
};

const numOfItemsPerPage = 2;

$(document).ready(function () {
    $("#filters input:not(#text-search), #filters select").change(function (async) {
        //Filter project list from team selection
        filterProjectsForItemsList();
    });

    $("#filters #text-search, #filters #start-date-search, #filters #end-date-search").keyup(function () {
        queryParameters.searchText = $("#text-search").val();
        filterProjectsForItemsList();
    });

    if ($('#filters').length) {
        updateFilterParameters(queryParameters.paginationStartItem);
    }
});


function updateFilterParameters(page) {
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
    queryParameters.startDate = $("#start-date-search").val();
    queryParameters.endDate = $("#end-date-search").val();
    console.log(page);
    queryParameters.paginationStartItem = (page * numOfItemsPerPage);
    filterItemList();
};

function filterItemList() {
    let stringifiedParameters = JSON.stringify(queryParameters);
    $.ajax({
        type: "GET",
        url: "/Item/GetFilteredItems",
        data: { "itemQueryJson": stringifiedParameters },
        success: function (data) {
            let itemListHtml = "";
            for (let i = 0; i < data.items.length; i++) {
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
            };
            $("#published-item-list").html(itemListHtml);

            //Pagination
            let itemListPaginationHtml = "";
            let numOfPages = 0;
            if (data.numOfTotalResults <= numOfItemsPerPage) {
                numOfPages = 0;
            } else {
                numOfPages = Math.ceil(data.numOfTotalResults / numOfItemsPerPage) - 1;
            }

            for (let i = 0; i <= numOfPages; i++) {
                itemListPaginationHtml += `<button class="pagination-item" onclick="updateFilterParameters(${i})">${i + 1}</button>`;
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
            updateFilterParameters(queryParameters.paginationStartItem);
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