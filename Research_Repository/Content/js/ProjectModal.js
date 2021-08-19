$(document).ready(function () {


    // When the user clicks on close button, close the modal
    $(".projectModalClose").click(function () {
        $("#myProjectModal").hide();
    });

    //On project modal button click
    $(".project-modal-btn").click(function () {
        updateProjects($(this));
    });

    ////If user clicks outside the modal
    $(document).mouseup(function (e) {
        const modal = $("#myProjectModal");
    if (modal.is(":visible")) {
            // if the target of the click isn't the container nor a descendant of the container
            if (modal.is(e.target) && modal.has(e.target).length === 0) {
                modal.hide();
                $("#project-name-input").val("");
            }
    }
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
            var $el = $("#project-teamId-modal-input");
            $('#project-teamId-modal-input option:gt(0)').remove(); // remove all options, but not the first

            for (let i = 0; i < data.length; i++) {
                $el.append($("<option></option>").attr("value", data[i].value).text(data[i].text));
            }
            
            $("#project-teamId-modal-input").val(teamId);

        },
        error: function (error) {
            console.log(error);
            alert("An error occurred!!!")
        }
    });
}

//Update projects
function updateProjects($this) {
    const $modal = $this.closest(".project-modal");
    const projectId = $modal.find("#project-id-modal-input").first().val();
    const teamId = $modal.find("#project-teamId-modal-input").find(":selected").attr("value");
    const oldTeamId = $modal.find("#project-oldTeamId-modal-input").val();
    const projectName = $modal.find("#project-name-modal-input").first().val();
    const formAction = $this.val();

    $.ajax({
        type: "GET",
        url: "/Project/UpdateProject",
        data: { "id": projectId, "projectName": projectName, "teamId": teamId, "oldTeamId": oldTeamId, "actionName": formAction },
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
    window.location.replace('../team?redirect=True');
}

function openProjectModal(project) {
    $("#project-delete-button").show().prop('disabled', false);
    $("#project-submit-button").val("Update");
    const projectName = $(project).siblings(".project-name").first().val();
    const projectId = $(project).siblings(".project-id").first().val();
    const teamId = $(project).closest(".team").find(".team-id").first().val();
    const oldTeamId = $(project).siblings("#project-oldTeamId-modal-input").first().val();
    $("#project-name-modal-input").attr("value", projectName);
    $("#project-id-modal-input").attr("value", projectId);
    $("#project-teamId-modal-input").val(teamId);
    $("#project-oldTeamId-modal-input").val(teamId);
    $("#myProjectModal").show();
    saveTempTeams(getTeams(), teamId);
    event.preventDefault();
}

//On add project click
function openAddProjectModal(project) {
    console.log("kk");
    $("#project-delete-button").hide().prop('disabled', true);
    $("#project-submit-button").val("Add");
    const teamId = $(project).closest(".team").find(".team-id").first().val();
    const oldTeamId = $(project).siblings("#project-oldTeamId-modal-input").first().val();
    $("#project-teamId-modal-input").val(teamId);
    $("#project-oldTeamId-modal-input").val(teamId);
    $("#project-id-modal-input").attr("value", 0);
    $("#project-name-modal-input").attr("value", "");
    $("#myProjectModal").show();
    saveTempTeams(getTeams(), teamId);
};