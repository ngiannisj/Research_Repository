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
            if (actionName == "Add" && id == 0)
            {
                //Team team = _projectRepo.GetTeam(teamId);
                //if (team != null)
                //{
                //    Project project = new Project { Name = projectName, TeamId = teamId };
                //    //If project is saved to database
                //    project.Name = projectName;
                //    project.TeamId = teamId;
                //    _projectRepo.Add(project);
                //    _projectRepo.Save();
                //}
                IList<Team> tempTeams = TempData.Get<IList<Team>>("teams");
                IList<Project> tempProjects = new List<Project>();
                Team newTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
                foreach (Team tempTeam in tempTeams)
                {
                    foreach (Project tempProject in tempTeam.Projects)
                    {
                        tempProjects.Add(tempProject);
                    }
                }

                int newId = 0;
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
                TempData.Put("teams", tempTeams);
                TempData.Keep();

            }
            else if (actionName == "Update" && id != 0)
            {
                Project project = _projectRepo.Find(id);
                if (project != null)
                {
                    ////If project is saved to database
                    ////If team exists in database
                    //if(_projectRepo.GetTeam(teamId) != null)
                    //{
                    //    project.Name = projectName;
                    //    project.TeamId = teamId;
                    //    _projectRepo.Update(project);
                    //    _projectRepo.Save();
                    //} else
                    //{
                    //    //Generate warning to save new team before changing project location
                    //    return;
                    //}

                }
                IList<Team> tempTeams = TempData.Get<IList<Team>>("teams");
                Team tempTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
                Team oldTeam = tempTeams.FirstOrDefault(u => u.Id == oldTeamId);
                Project tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == id);
                if (tempProject == null)
                {
                    tempTeam.Projects.Add(new Project { Id = id, Name = projectName, TeamId = teamId });
                    oldTeam.Projects.Remove(oldTeam.Projects.FirstOrDefault(u => u.Id == id));
                    tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == id);
                }
                tempProject.Name = projectName;
                TempData.Put("teams", tempTeams);
                TempData.Keep();

            }
            else if (actionName == "Delete")
            {
                //Project project = _projectRepo.Find(id);
                //if (project != null)
                //{
                //    //If project is saved to database
                //    _projectRepo.Remove(project);
                //    _projectRepo.Save();
                //}
                IList<Team> tempTeams = TempData.Get<IList<Team>>("teams");
                Team tempTeam = tempTeams.FirstOrDefault(u => u.Id == teamId);
                Project tempProject = tempTeam.Projects.FirstOrDefault(u => u.Id == id);
                tempTeam.Projects.Remove(tempProject);
                TempData.Put("teams", tempTeams);
                TempData.Keep();
            }
        }

    }
}
