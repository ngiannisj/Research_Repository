let queryParameters = {
    searchText = "*",
    themes =[],
    teams =[],
    projects =[],
    tags =[],
    sensitivity =[],
    approvals =[],
    startDate = "*",
    endDate = "*"
};

$(document).ready(function () {

    $("#filters input:not(#text-search), #filters select").change(function (async) {
        //Filter project list from team selection
        filterProjectsForItemsList();
    });

    $("#filters #text-search, #filters #start-date-search, #filters #end-date-search").keypress(function () {
        queryParameters.searchText = $("#text-search").val();
        /*filterItemList();*/
    });
});

function updateFilterParameters() {
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
    console.log(queryParameters);
};

function filterItemList() {
    let stringifiedParameters = JSON.stringify(queryParameters);
    $.ajax({
        type: "GET",
        url: "/Item/GetFilteredItems",
        data: { "itemQueryJson": stringifiedParameters },
        success: function (data) {
            //show filtered items
            console.log(data);
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
            //filterItemList();
            updateFilterParameters();
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