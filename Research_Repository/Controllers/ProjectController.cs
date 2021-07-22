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

        public IActionResult Index()
        {
            IEnumerable<Project> objList = _projectRepo.GetAll();
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ProjectVM projectVM = _projectRepo.GetProjectVM();
            if (id == null)
            {
                //this is for create
                return View(projectVM);
            }
            else
            {
                projectVM.Project = _projectRepo.FirstOrDefault(filter: u => u.Id == id, isTracking: false);
                if (projectVM.Project == null)
                {
                    return NotFound();
                }
                return View(projectVM);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProjectVM projectVM)
        {

            if (ModelState.IsValid)
            {

                if(projectVM.Project.Id == 0)
                {
                    _projectRepo.Add(projectVM.Project);
                    _projectRepo.Save();
                } else
                {
                    //Updating
                    var objFromDb = _projectRepo.FirstOrDefault(filter: u => u.Id == projectVM.Project.Id, isTracking: false);
                    _projectRepo.Update(projectVM.Project);
                }

                _projectRepo.Save();
                return RedirectToAction("Index");
            }
            return View(projectVM.Project);
        }

        //DELETE - DELETE
        public int Delete(int id)
        {
            var obj = _projectRepo.Find(id);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                if (!_projectRepo.HasItems(obj.Id))
                {
                    _projectRepo.Remove(obj);
                    _projectRepo.Save();
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
