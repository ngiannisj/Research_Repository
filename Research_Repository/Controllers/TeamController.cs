using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class TeamController : Controller
    {

        private readonly ITeamRepository _teamRepo;

        public TeamController(ITeamRepository teamRepo)
        {
            _teamRepo = teamRepo;
        }

        public IActionResult Index(IList<Team> teams, bool redirect)
        {
            if (teams.Count == 0)
            {
                if (redirect == false)
                {
                    IEnumerable<Team> objList = _teamRepo.GetAll();
                    foreach (Team obj in objList)
                    {
                        obj.Projects = _teamRepo.GetTeamProjectsFromDb(obj.Id);
                    }
                    IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(objList);
                    TempData.Put("teamSelectList", teamsSelectList);
                    return View(objList);
                }
                else
                {
                    IList<Team> tempTeams = TempData.Get<IList<Team>>("teams");
                    

                    foreach (Team team in tempTeams)
                    {
                        if(team.Projects == null)
                        {
                            team.Projects = new List<Project>();
                        }
                    }
                    IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(tempTeams);
                    TempData.Put("teamSelectList", teamsSelectList);
                    TempData.Keep();
                    ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                    return View(tempTeams);
                }
            }
            else
            {
                IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(teams);
                TempData.Put("teamSelectList", teamsSelectList);
                return View(teams);
            };
        }

        public IEnumerable<SelectListItem> SaveTeamsState([FromBody] IList<Team> teams)
        {
            IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(teams);
            TempData.Put("teamSelectList", teamsSelectList);
            TempData.Put("teams", teams);
            return teamsSelectList;
        }

        public IActionResult SaveTeams(IList<Team> teams)
        {
            teams = TempData.Get<IList<Team>>("teams");
            IList<int> tempTeamIdList = _teamRepo.GetTeamIds(teams);
            IEnumerable<Team> dbTeamList = _teamRepo.GetAll(isTracking: false, includeProperties: "Projects");
            IList<int> dbTeamIdList = _teamRepo.GetTeamIds(dbTeamList);


            foreach (Team team in teams)
            {
                if (!dbTeamIdList.Contains(team.Id))
                {
                    team.Id = 0;
                    IList<Project> teamProjects = team.Projects;
                    team.Projects = null;
                    _teamRepo.Add(team);
                    _teamRepo.Save();
                    team.Projects = teamProjects;
                }
            
            dbTeamList = _teamRepo.GetAll(isTracking: false, includeProperties: "Projects");
            dbTeamIdList = _teamRepo.GetTeamIds(dbTeamList);
                    if (dbTeamIdList.Contains(team.Id))
                    {
                        //Update projects db
                        IList<Project> projects = team.Projects;
                        _teamRepo.UpsertProjects(team.Id, projects);
                        IList<int> tempProjectIdList = _teamRepo.GetProjectIds(teams, false);
                        //Remove projects from db if they do not exist in temp data
                        _teamRepo.DeleteProjects(tempProjectIdList);
                        _teamRepo.Attach(team);
                        _teamRepo.Save();
                    
            }
        }

            //Reevaluate lists after adding and updating teams/projects
            dbTeamList = _teamRepo.GetAll(isTracking: false, includeProperties: "Projects");
            tempTeamIdList = _teamRepo.GetTeamIds(teams);
            foreach (Team obj in dbTeamList)
            {
                if (!tempTeamIdList.Contains(obj.Id))
                {
                    _teamRepo.Remove(obj);
                }
            }
            
            _teamRepo.Save();
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            return RedirectToAction("Index");
        }

        public IActionResult AddTeam(IList<Team> teams)
        {
            int newId = 0;
            IList<Team> teamList = TempData.Get<IList<Team>>("teams");
            if(teamList == null)
            {
                teamList = teams;
            }

            if (teamList.Count > 0)
            {
                newId = teamList[teamList.Count - 1].Id + 1;
            }
            teams.Add(new Team{ Id = newId, Name = "", Projects = new List<Project>() });

            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveTeamsState(teams);
            return RedirectToAction("Index", new { redirect = true });
        }

        public IActionResult DeleteTeam(IList<Team> teams, int deleteId)
        {
            IList<Project> tempProjects = _teamRepo.GetProjectsFromTeams(teams);
            Team itemToRemove = teams.FirstOrDefault(u => u.Id == deleteId);
            if (!_teamRepo.HasProjects(itemToRemove.Id, tempProjects))
            {
                teams.Remove(itemToRemove);
            }
            else
            {
                //Give warning to not delete until items are removed
            }
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveTeamsState(teams);
            return RedirectToAction("Index", new { redirect = true });
        }

    }
}
