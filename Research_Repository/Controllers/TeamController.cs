﻿using Microsoft.AspNetCore.Authorization;
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
                    return View(objList);
                }
                else
                {
                    IList<Team> tempTeams = TempData.Get<IList<Team>>("key");
                    TempData.Keep();
                    ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                    return View(tempTeams);
                }
            }
            else
            {
                return View(teams);
            };
        }

        public void SaveTeamsState([FromBody] IList<Team> teams)
        {
            TempData.Put("key", teams);
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
                    _teamRepo.Update(obj);
                }
                else
                {
                    obj.Id = 0;
                    _teamRepo.Add(obj);
                }
            }
            _teamRepo.Save();
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            return RedirectToAction("Index");
        }

        public IActionResult AddTeam(IList<Team> teams)
        {
            int newId = 0;
            if (teams.Count > 0)
            {
                newId = teams[teams.Count - 1].Id + 1;
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


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            Team team = new Team();
            if (id == null)
            {
                //this is for create
                return View(team);
            }
            else
            {
                team = _teamRepo.FirstOrDefault(filter: u => u.Id == id, isTracking: false);
                if (team == null)
                {
                    return NotFound();
                }
                return View(team);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Team obj)
        {

            if (ModelState.IsValid)
            {

                if(obj.Id == 0)
                {
                    _teamRepo.Add(obj);
                    _teamRepo.Save();
                } else
                {
                    //Updating
                    var objFromDb = _teamRepo.FirstOrDefault(filter: u => u.Id == obj.Id, isTracking: false);
                    _teamRepo.Update(obj);
                }

                _teamRepo.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //DELETE - DELETE
        public int Delete(int id)
        {
            var obj = _teamRepo.Find(id);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                if (!_teamRepo.HasProjects(obj.Id))
                {
                    _teamRepo.Remove(obj);
                    _teamRepo.Save();
                    return 2;
                }
                else
                {
                    return 3;
                }

            }
        }

    }
}
