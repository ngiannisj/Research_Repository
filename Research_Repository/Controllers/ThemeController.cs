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
    public class ThemeController : Controller
    {

        private readonly IThemeRepository _themeRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ThemeController(IThemeRepository themeRepo, IWebHostEnvironment webHostEnvironment)
        {
            _themeRepo = themeRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Theme> objList = _themeRepo.GetAll();
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
           ThemeVM themeVM = _themeRepo.CreateThemeVM(id);

            if (id == null)
            {
                themeVM.Theme = new Theme();
                //this is for create
                return View(themeVM);
            }
            else
            {
                themeVM.Theme = _themeRepo.FirstOrDefault(filter: u => u.Id == id, isTracking: false);
                if (themeVM.Theme == null)
                {
                    return NotFound();
                }
                return View(themeVM);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ThemeVM obj)
        {

            if (ModelState.IsValid)
            {
                 var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(obj.Theme.Id == 0)
                {
                    //Creating
                    if (files.Count != 0) {
                        //Upload image
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Theme.Image = fileName + extension;
                    }

                    _themeRepo.Add(obj.Theme);
                    _themeRepo.Save();
                    _themeRepo.UpdateThemeTagsList(obj);
                } else
                {
                    //Updating
                    var objFromDb = _themeRepo.FirstOrDefault(filter: u => u.Id == obj.Theme.Id, isTracking: false);

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        if(objFromDb.Image != null)
                        {
                            var oldFile = Path.Combine(upload, objFromDb.Image);

                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Theme.Image = fileName + extension;
                    }
                    else
                    {
                        obj.Theme.Image = objFromDb.Image;
                    }
                    _themeRepo.Update(obj.Theme);
                    _themeRepo.UpdateThemeTagsList(obj);
                }

                _themeRepo.Save();
                return RedirectToAction("Index");
            }
            return View(obj.Theme);
        }


        //DELETE - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _themeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }else
            {
                if (obj.Image != null)
                {

                    string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

                    var oldFile = Path.Combine(upload, obj.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }
            }
            _themeRepo.Remove(obj);
            _themeRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
