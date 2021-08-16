$(document).ready(function () {

    //// When the user clicks the open button, open the modal 
    //$(".myProjectBtn").click(function () {

    //    event.preventDefault();
    //});

    // When the user clicks on close button, close the modal
    $(".projectModalClose").click(function () {
        $("#myProjectModal").hide();
    });

    //On add project click
    $(".addProjectModalBtn").click(function () {
        $("#project-delete-button").hide().prop('disabled', true);
        $("#project-submit-button").val("Add");
        const teamId = $(this).closest(".team").find(".team-id").first().val();
        $("#project-teamId-modal-input").attr("value", teamId);
        $("#project-id-modal-input").attr("value", 0);
        $("#project-name-modal-input").attr("value", "");
        $("#myProjectModal").show();
        saveState(getTeams());
    });

    $(".project-modal-btn").click(function () {
        console.log("th");
        updateProjects($(this));
    });

});

function getTeams() {
    let teamsList = [];

    $(".team").each(function (index, element) {
        let projectsList = [];

        const teamId = $(element).find(".team-id").first().val();
        const teamName = $(element).find(".team-name").first().val();

        $(element).find(".project").each(function (i, e) {
            const projectId = $(e).find(".project-id").first().val();
            const projectName = $(e).find(".project-name").first().val();
            const project = { Id: projectId, Name: projectName, TeamId: teamId }
            projectsList.push(project);
        })
        const team = { Id: teamId, Name: teamName, Projects: projectsList }
        teamsList.push(team);
    });
    return teamsList;
}

function saveState(teamsList) {
    let tempTeams = JSON.stringify(teamsList);
    $.ajax({
        type: "POST",
        url: "/Team/SaveTeamsState",
        traditional: true,
        data: tempTeams,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}

//Update projects
function updateProjects($this) {
    const $modal = $this.closest(".project-modal");
    const projectId = $modal.find("#project-id-modal-input").first().val();
    const teamId = $modal.find("#project-teamId-modal-input").first().val();
    const projectName = $modal.find("#project-name-modal-input").first().val();
    const formAction = $this.val();

    $.ajax({
        type: "GET",
        url: "/Project/UpdateProject",
        data: { "id": projectId, "projectName": projectName, "teamId": teamId, "actionName": formAction },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
                updateTeamProjects(teamId);
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}

function updateTeamProjects() {
    $.ajax({
        type: "GET",
        url: "/Team/UpdateTeamProjects",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            window.location.replace('../team?redirect=True');
        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}

function openProjectModal(project) {
    $("#project-delete-button").show().prop('disabled', false);
    $("#project-submit-button").val("Update");
    const projectName = $(project).siblings(".project-name").first().val();
    const projectId = $(project).siblings(".project-id").first().val();
    const teamId = $(project).closest(".team").find(".team-id").first().val();
    $("#project-name-modal-input").attr("value", projectName);
    $("#project-id-modal-input").attr("value", projectId);
    $("#project-teamId-modal-input").attr("value", teamId);
    $("#myProjectModal").show();
    saveState(getTeams());
    event.preventDefault();
}