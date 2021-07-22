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
    public class TeamController : Controller
    {

        private readonly ITeamRepository _teamRepo;

        public TeamController(ITeamRepository teamRepo)
        {
            _teamRepo = teamRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Team> objList = _teamRepo.GetAll();
            return View(objList);
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
