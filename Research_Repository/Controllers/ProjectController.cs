using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public void UpdateProject(int id, string projectName, int teamId, int oldTeamId, string actionName)
        {
            IList<Project> tempProjects = new List<Project>();
            IList<Team> tempTeams = HttpContext.Session.Get<IList<Team>>("teams");

            Team newTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
            foreach (Team tempTeam in tempTeams)
            {
                foreach (Project tempProject in tempTeam.Projects)
                {
                    tempProjects.Add(tempProject);
                }
            }

            if (actionName == "Add" && id == 0)
            {
                int newId = 1;
                if (tempProjects != null && tempProjects.Count > 0)
                {
                    newId = tempProjects.Select(u => u.Id).ToList().Max() + 1;
                }
                else
                {
                    IEnumerable<int> projectList = _projectRepo.GetAll(isTracking: false).Select(u => u.Id);
                    if (projectList.Count() > 0)
                    {
                        newId = projectList.Last() + 1;
                    }
                }

                newTeam.Projects.Add(new Project { Id = newId, Name = projectName, TeamId = teamId });
                HttpContext.Session.Set("teams", tempTeams);

            }
            else if (actionName == "Update")
            {
                tempTeams = HttpContext.Session.Get<IList<Team>>("teams");
                Team tempTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
                Team oldTeam = tempTeams.FirstOrDefault(u => u.Id == oldTeamId);
                int newId = tempProjects.Select(u => u.Id).ToList().Max() + 1;
                Project tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == id);
                if (tempProject == null)
                {
                    tempTeam.Projects.Add(new Project { Id = newId, Name = projectName, TeamId = teamId });
                    oldTeam.Projects.Remove(oldTeam.Projects.FirstOrDefault(u => u.Id == id));
                    tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == newId);
                }
                tempProject.Name = projectName;
                HttpContext.Session.Set("teams", tempTeams);

            }
            else if (actionName == "Delete")
            {
                tempTeams = HttpContext.Session.Get<IList<Team>>("teams");
                Team tempTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
                Project tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == id);
                tempTeam.Projects.Remove(tempProject);
                HttpContext.Session.Set("teams", tempTeams);
            }
        }

    }
}
