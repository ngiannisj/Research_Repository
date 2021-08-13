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

        public IActionResult Index(IList<ThemeVM> themes, string save, string add, int? delete, bool redirect)
        {
            IEnumerable<Theme> themeList = _themeRepo.GetAll(isTracking: false);
            IList<Theme> themeListFromThemeVM = new List<Theme>();

            if (themes.Count == 0)
            {
                if(redirect == false)
                {
                    //If no form button is clicked
                    IList<ThemeVM> themeVMList = new List<ThemeVM>();
                    foreach (Theme theme in themeList)
                    {
                        themeVMList.Add(_themeRepo.CreateThemeVM(null, theme.Id));
                    }
                    return View(themeVMList);
                } else
                {

                    IList<ThemeVM> tempThemes = TempData.Get<IList<ThemeVM>>("key");
                    TempData.Keep();
                    return View(tempThemes);
                }
                
            }

            if (!string.IsNullOrEmpty(save))
            {

                IEnumerable<Theme> dbObjList = _themeRepo.GetAll(isTracking: false);

                foreach (Theme obj in dbObjList)
                {
                    if (!themeListFromThemeVM.Contains(obj))
                    {
                        _themeRepo.Remove(obj);
                    }
                }
                foreach (ThemeVM obj in themes)
                {

                    if (dbObjList.Contains(obj.Theme))
                    {
                        _themeRepo.Update(obj.Theme);
                    }
                    else
                    {
                        obj.Theme.Id = 0;
                        _themeRepo.Add(obj.Theme);
                        _themeRepo.Save();
                        _themeRepo.UpdateThemeTagsList(obj);
                    }


                    //if (obj.Theme.Id == 0)
                    //{
                    //    //Creating
                    //    //if (files.Count != 0)
                    //    //{
                    //    //    //Upload Image
                    //    //    obj.Theme.Image = FileHelper.UploadFiles(files, webRootPath, WC.ImagePath);
                    //    //}


                    //}
                    //else
                    //{
                    //    //Updating
                    //    //var objFromDb = _themeRepo.FirstOrDefault(filter: u => u.Id == obj.Theme.Id, isTracking: false);

                    //    //if (files.Count > 0)
                    //    //{

                    //    //    if (objFromDb.Image != null)
                    //    //    {
                    //    //        List<string> filesArray = objFromDb.Image.Split(',').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
                    //    //        FileHelper.DeleteFile(webRootPath, filesArray[0], WC.ImagePath);
                    //    //    }

                    //    //    obj.Theme.Image = FileHelper.UploadFiles(files, webRootPath, WC.ImagePath);
                    //    //}
                    //    //else
                    //    //{
                    //    //    obj.Theme.Image = objFromDb.Image;
                    //    //}
                    //    _themeRepo.Update(obj.Theme);
                    //    _themeRepo.UpdateThemeTagsList(obj);
                    //}

                }
                foreach (ThemeVM obj in themes)
                {
                    //Get themes from themeVM
                    themeListFromThemeVM.Add(obj.Theme);
                }
                _themeRepo.Save();
                ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                return RedirectToAction("Index");
            }
            else if (!string.IsNullOrEmpty(add))
            {
                string newId = "0";
                if (themes.Count > 0)
                {
                    newId = themes[themes.Count - 1].Theme.Id + 1.ToString();
                }
                themes.Add(_themeRepo.CreateThemeVM(newId, null));
                foreach (ThemeVM obj in themes)
                {
                    //Get themes from themeVM
                    themeListFromThemeVM.Add(obj.Theme);
                }
                ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                TempData.Put("key", themes);
                return RedirectToAction("Index", new { redirect = true });
            }
            else if (delete != null)
            {
               ThemeVM itemToRemove = themes.FirstOrDefault(u => u.Theme.Id == delete);
                if (!_themeRepo.HasItems(itemToRemove.Theme.Id))
                {
                    themes.Remove(itemToRemove);
                } else
                {
                    //Give warning to not delete until items are removed
                }
                ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                TempData.Put("key", themes);
                return RedirectToAction("Index", new { redirect = true });
            }
            else
            {
                return RedirectToAction("Index", themes);
            };
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ThemeVM themeVM = _themeRepo.CreateThemeVM(null, id);
            if (id == null)
            {

                //this is for create
                return View(themeVM);
            }
            else
            {
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

                if (obj.Theme.Id == 0)
                {
                    //Creating
                    if (files.Count != 0)
                    {
                        //Upload Image
                        obj.Theme.Image = FileHelper.UploadFiles(files, webRootPath, WC.ImagePath);
                    }

                    _themeRepo.Add(obj.Theme);
                    _themeRepo.Save();
                    _themeRepo.UpdateThemeTagsList(obj);
                }
                else
                {
                    //Updating
                    var objFromDb = _themeRepo.FirstOrDefault(filter: u => u.Id == obj.Theme.Id, isTracking: false);

                    if (files.Count > 0)
                    {

                        if (objFromDb.Image != null)
                        {
                            List<string> filesArray = objFromDb.Image.Split(',').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
                            FileHelper.DeleteFile(webRootPath, filesArray[0], WC.ImagePath);
                        }

                        obj.Theme.Image = FileHelper.UploadFiles(files, webRootPath, WC.ImagePath);
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
        public int Delete(int id)
        {
            //var obj = _themeRepo.Find(id);
            //if (obj == null)
            //{
            //    return 1;
            //}
            //else
            //{
            //    //If there are no items associated with this theme
            //    if (!_themeRepo.HasItems(obj.Id))
            //    {
            //        //if (obj.Image != null)
            //        //{
            //        //    string webRootPath = _webHostEnvironment.WebRootPath;

            //        //    List<string> filesArray = obj.Image.Split(',').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
            //        //    FileHelper.DeleteFile(webRootPath, filesArray[0], WC.ImagePath);
            //        //}
            //        //_themeRepo.Remove(obj);
            //        //_themeRepo.Save();
            //        return 2;
            //    }
            //    else
            //    {
            //        return 3;
            //    }

            //}
            return 4;
        }

        //GET - DOWNLOAD FILE
        public IActionResult GetDownloadedFile(string filePath)
        {
            return FileHelper.DownloadFile(filePath);
        }
    }
}
