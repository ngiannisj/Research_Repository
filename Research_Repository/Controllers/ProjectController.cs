using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Utility;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.LibrarianRole)]
    public class ProjectController : Controller
    {

        private readonly IProjectRepository _projectRepo;

        public ProjectController(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        //POST - UPDATE
        //Update 'tempTeams' object in session with added/updated/deleted project
        public void UpdateProject(int id, string projectName, int teamId, int oldTeamId, string actionName)
        {
            //Get list of temp teams from the session
            IList<Team> tempTeams = HttpContext.Session.Get<IList<Team>>("teams");

            //Get team object from session based on teamId provided in action parameter
            Team team = new Team();
            if(tempTeams != null && tempTeams.Count > 0)
            {
                team = tempTeams.FirstOrDefault(u => u.Id == teamId);
            }

            //Get a list of all projects for all teams stored in the session
            IList<Project> tempProjects = new List<Project>();
            if(tempTeams != null && tempTeams.Count > 0)
            {
                foreach (Team tempTeam in tempTeams)
                {
                    if(tempTeam.Projects != null && tempTeam.Projects.Count > 0)
                    {
                        foreach (Project project in tempTeam.Projects)
                        {
                            tempProjects.Add(project);
                        }
                    }
                }
            }

            //Get selected project from newly assigned team
            Project tempProject = null;
            if (team != null && team.Projects.Count > 0)
            {
                tempProject = team.Projects.FirstOrDefault(u => u.Id == id);
            }

            //Generate a unique id for a newly added or updated project
            int newId = 1;
            //Get next available id for the project if projects exist in 'tempTeams'
            if (tempProjects != null && tempProjects.Count > 0)
            {
                //Get max id value of all projects in 'tempTeams' and add 1
                newId = tempProjects.Select(u => u.Id).ToList().Max() + 1;
            }
            else
            {
                //If no projects exist in 'tempTeams' get next available id value from database
                IEnumerable<int> projectList = _projectRepo.GetAll(isTracking: false).Select(u => u.Id);
                if (projectList.Count() > 0)
                {
                    newId = projectList.Last() + 1;
                }
                //If no projects exist in database value of 'newId' stays at 1
            }

            //Add new project to the desired team in 'tempTeams'
            if (actionName == "Add" && id == 0)
            {
                //Add new project item to list of projects in team
                team.Projects.Add(new Project { Id = newId, Name = projectName, TeamId = teamId });
            }

            //Update project in 'tempTeams'
            else if (actionName == "Update")
            {
                //Get team of project when modal was initially loaded
                Team oldTeam = tempTeams.FirstOrDefault(u => u.Id == oldTeamId);

                //If no projects exist in the newly assigned team, add the project to the team and remove it from the old team
                if (tempProject == null)
                {
                    //Add project to new team
                    team.Projects.Add(new Project { Id = newId, Name = projectName, TeamId = teamId });
                    //Remove project from old team
                    oldTeam.Projects.Remove(oldTeam.Projects.FirstOrDefault(u => u.Id == id));

                    tempProject = team.Projects.FirstOrDefault(u => u.Id == newId);
                }
                //Assign new name to the project
                tempProject.Name = projectName;

            }
            else if (actionName == "Delete")
            {
                //Remove project from team in 'tempTeams'
                if(team !=null && team.Projects.Count > 0)
                {
                    team.Projects.Remove(tempProject);
                }
            }

            //Update 'tempTeams' in session
            HttpContext.Session.Set("teams", tempTeams);
        }

    }
}
