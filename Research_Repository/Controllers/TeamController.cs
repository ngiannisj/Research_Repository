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
                        obj.Projects = _teamRepo.GetTeamProjects(obj.Id);
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
            IList<int> teamIdListFromTeam = _teamRepo.GetTeamIds(teams);
            IEnumerable<Team> dbObjList = _teamRepo.GetAll(isTracking: false, includeProperties: "Projects");
            IList<int> dbObjIdList = _teamRepo.GetTeamIds(dbObjList);

            foreach (Team obj in dbObjList)
            {
                if (!teamIdListFromTeam.Contains(obj.Id))
                {
                    _teamRepo.Remove(obj);
                }
            }
            foreach (Team obj in teams)
            {
                if (dbObjIdList.Contains(obj.Id))
                {
                    _teamRepo.Attach(obj);
                }
                else
                {
                    obj.Id = 0;
                    IList<Project> projects = obj.Projects;
                    obj.Projects = null;
                    _teamRepo.Add(obj);
                    _teamRepo.Save();
                    _teamRepo.AddProjects(obj.Id, projects);
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
            Team itemToRemove = teams.FirstOrDefault(u => u.Id == deleteId);
            if (!_teamRepo.HasProjects(itemToRemove.Id))
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
