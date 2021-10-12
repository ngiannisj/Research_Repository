$(document).ready(function () {

    //On modal action button click e.g. delete, save
    $(".project-modal-btn").click(function () {
        updateProjects($(this));
        event.preventDefault();
    });

    // On modal close hide modal
    $("#close-add-project-modal-button").click(function () {
        $("#project-name-modal-input").val("");
        $("#myProjectModal").addClass("hidden");
        $("body").removeClass("no-scroll");
        $("#selected-project-name-error-text").addClass("hidden");
        $("#project-name-modal-input").removeClass("error");
        event.preventDefault();
    });

    $(document).mouseup(function (e) {
        const modal = $("#myProjectModal");
        if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                $("#project-name-modal-input").val("");
                modal.addClass("hidden");
                $("body").removeClass("no-scroll");
                $("#selected-project-name-error-text").addClass("hidden");
                $("#project-name-modal-input").removeClass("error");
            }
        }
    });

    //======================================================================
    //These ajax calls are here because there is too much data to pass through the url without compromising security

        //Delete team from session teams
    $("#delete-team-confirm-btn").click(function () {
        event.preventDefault();
        const teamId = $(this).val();
        const teamsList = getTeams();
        const jsonTeamsList = JSON.stringify(teamsList);
        $.ajax({
            type: "POST",
            url: "/Team/DeleteTeam",
            data: { "teamsListString": jsonTeamsList, "deleteId": teamId },
            success: function (data) {
              //Reload teams page with new teams list
                window.location.replace('../team?redirect=True');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    $($("#selected-team-name-input")).on("input", function () {
        if ($(this).hasClass("error")) {
            $("#selected-team-name-error-text").addClass("hidden");
            $("#selected-team-name-input").removeClass("error");
        }
    });

        //Add team to session teams
    $("#add-team-submit-button").click(function () {

        if (!$("#selected-team-name-input").val()) {
            $("#selected-team-name-error-text").removeClass("hidden");
            $("#selected-team-name-input").addClass("error");
            return
        }

        event.preventDefault();
        const name = $("#selected-team-name-input").val();
        const contact = $("#selected-team-contact-input").val();
        const teamsList = getTeams();
        const jsonTeamsList = JSON.stringify(teamsList);
        $.ajax({
            type: "POST",
            url: "/Team/AddTeam",
            data: { "teamsListString": jsonTeamsList, "teamName": name, "teamContact": contact },
            success: function (data) {
                //Reload teams page with new teams list
                window.location.replace('../team?redirect=True');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    //Save teams to database
    $("#save-teams-btn").click(function () {

        event.preventDefault();
        const teamsList = getTeams();
        const jsonTeamsList = JSON.stringify(teamsList);
        $.ajax({
            type: "POST",
            url: "/Team/SaveTeams",
            data: { "teamsListString": jsonTeamsList },
            success: function (data) {
                //Reload teams page with new teams list
                window.location.replace('../team');
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
    //=============================================================================

});

//Get list of teams from DOM
function getTeams() {
    let teamsList = [];

    $(".team").each(function (index, element) {
        let projectsList = [];

        const teamId = $(element).find(".team-id").first().val();
        const teamName = $(element).find(".team-name-input").first().val();
        const teamContact = $(element).find(".team-contact-input").first().val();

        $(element).find(".project").each(function (i, e) {
            const projectId = $(e).find(".project-id").first().val();
            const projectName = $(e).find(".project-name-input").first().val();
            const project = { Id: projectId, Name: projectName, TeamId: teamId }
            projectsList.push(project);
        })
        const team = { Id: teamId, Name: teamName, Contact: teamContact, Projects: projectsList, Users: [] };
        teamsList.push(team);
    });
    return teamsList;
}

//Save temp teams to session
function saveTempTeams(teamsList, teamId) {
    let tempTeams = JSON.stringify(teamsList);
    $.ajax({
        type: "POST",
        url: "/Team/SaveTeamsState",
        traditional: true,
        data: tempTeams,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            //Reload team dropdown list
            let $el = $(".accordion__content--select-list").first();
                $('#project-teamId-modal-input').html(""); // remove all options, but not the first two
                let options = "";

                for (let i = 0; i < data.length; i++) {
                    options += `<button onclick="selectListOptionClick(this)"
                                    class="accordion__content-option"
                                    data-value="${data[i].value}">${data[i].text}
                            </button>`
                }
            $("#project-teamId-modal-input").val(teamId);
            $($el).html(options);

        },
        error: function (error) {
            console.log(error);
        }
    });
}

$($("#project-name-modal-input")).on("input", function () {
    if ($(this).hasClass("error")) {
        $("#selected-project-name-error-text").addClass("hidden");
        $("#project-name-modal-input").removeClass("error");
    }
});

//Update projects
function updateProjects($this) {

    if (!$("#project-name-modal-input").val()) {
        $("#selected-project-name-error-text").removeClass("hidden");
        $("#project-name-modal-input").addClass("error");
        return
    }

    const projectId = $("#project-id-modal-input").first().val();
    const teamId = $("#project-teamId-modal-input").val();
    const oldTeamId = $("#project-oldTeamId-modal-input").val();
    const projectName = $("#project-name-modal-input").first().val();
    const formAction = $this.val();

    $.ajax({
        type: "GET",
        url: "/Project/UpdateProject",
        data: { "id": projectId, "projectName": projectName, "teamId": teamId, "oldTeamId": oldTeamId, "actionName": formAction },
        contentType: 'application/json; charset=utf-8',
        success: function () {
            //Reload teams page
            updateTeamProjects(teamId);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//Reload teams page
function updateTeamProjects() {
    window.location.replace('../team?redirect=True');
}

//Open project modal
function openProjectModal(project) {
    $("#project-delete-button").show().prop('disabled', false);
    $("#project-submit-button").val("Update");
    const projectName = $(project).siblings(".project-name-input").first().val();
    const projectId = $(project).siblings(".project-id").first().val();
    const teamId = $(project).closest(".team").find(".team-id").first().val();
    const teamName = $(project).closest(".team").find(".team-name-input").first().val();
    $("#project-name-modal-input").val(projectName);
    $("#project-id-modal-input").attr("value", projectId);
    $("#project-teamId-modal-input").val(teamId);
    $("#project-teamId-modal-selectList").html(teamName);
    $("#project-oldTeamId-modal-input").val(teamId);
    $("body").addClass("no-scroll");
    $("#myProjectModal .modal__title").text("Edit a project");
    $("#myProjectModal").removeClass("hidden");
    findInsiders($("#myProjectModal"));
    saveTempTeams(getTeams(), teamId);
    event.preventDefault();
}

//On open project modal
function openAddProjectModal(project) {
    $("body").addClass("no-scroll");
    $("#open-project-delete-modal-btn-container").addClass("hidden");
    $("#project-submit-button").val("Add");
    const teamId = $(project).closest(".team").find(".team-id").first().val();
    const teamName = $(project).closest(".team").find(".team-name-input").first().val();
    $("#project-teamId-modal-input").val(teamId);
    $("#project-teamId-modal-selectList").html(teamName);
    $("#project-oldTeamId-modal-input").val(teamId);
    $("#project-id-modal-input").attr("value", 0);
    $("#project-name-modal-input").attr("value", "");
    $("#myProjectModal .modal__title").text("Add a project");
    $("#myProjectModal").removeClass("hidden");
    findInsiders($("#myProjectModal"));
    saveTempTeams(getTeams(), teamId);
    event.preventDefault();
};