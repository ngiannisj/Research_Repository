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

        public IActionResult Index(IList<ThemeVM> themes, bool redirect)
        {
            IEnumerable<Theme> themeList = _themeRepo.GetAll(isTracking: false);

            if (themes.Count == 0)
            {
                if (redirect == false)
                {
                    //If no form button is clicked
                    IList<ThemeVM> themeVMList = new List<ThemeVM>();
                    foreach (Theme theme in themeList)
                    {
                        themeVMList.Add(_themeRepo.CreateThemeVM(null, theme.Id));
                    }
                    //Update first theme model with the tag select dropdown list
                    themeVMList[0].TagSelectList = _themeRepo.GetTagList();
                    return View(themeVMList);
                }
                else
                {
                    IList<ThemeVM> tempThemes = TempData.Get<IList<ThemeVM>>("key");
                    TempData.Keep();
                    //Update first theme model with the tag select dropdown list
                    tempThemes[0].TagSelectList = _themeRepo.GetTagList();
                    ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
                    return View(tempThemes);
                }
            }
            else
            {
                return View(themes);
            };
        }

        public void SaveThemesState([FromBody]IList<ThemeVM> themes)
        {
            TempData.Put("key", themes);
        }

        public IActionResult SaveThemes(IList<ThemeVM> themes)
        {
            IList<int> themeIdListFromThemeVM = new List<int>();
            foreach (ThemeVM obj in themes)
            {
                //Get themes from themeVM
                themeIdListFromThemeVM.Add(obj.Theme.Id);
            }

            IEnumerable<Theme> dbObjList = _themeRepo.GetAll(isTracking: false);
            IList<int> dbObjIdList = _themeRepo.GetThemeIds(dbObjList);

            foreach (Theme obj in dbObjList)
            {
                if (!themeIdListFromThemeVM.Contains(obj.Id))
                {
                    _themeRepo.Remove(obj);
                }
            }
            foreach (ThemeVM obj in themes)
            {

                if (dbObjIdList.Contains(obj.Theme.Id))
                {
                    _themeRepo.UpdateThemeTagsList(obj);
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
            _themeRepo.Save();
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTheme(IList<ThemeVM> themes, int deleteId)
        {
            ThemeVM itemToRemove = themes.FirstOrDefault(u => u.Theme.Id == deleteId);
            if (!_themeRepo.HasItems(itemToRemove.Theme.Id))
            {
                themes.Remove(itemToRemove);
            }
            else
            {
                //Give warning to not delete until items are removed
            }
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveThemesState(themes);
            return RedirectToAction("Index", new { redirect = true });
        }

        public IActionResult AddTheme(IList<ThemeVM> themes)
        {
            IList<Theme> themeListFromThemeVM = new List<Theme>();
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
            SaveThemesState(themes);
            return RedirectToAction("Index", new { redirect = true });
        }

        public bool UpdateThemeTags()
        {
            IList<ThemeVM> tempThemes = TempData.Get<IList<ThemeVM>>("key");
            foreach(ThemeVM themeVM in tempThemes)
            {
                themeVM.TagCheckboxes = _themeRepo.GetTagCheckboxes(themeVM.Theme.Id, themeVM.TagCheckboxes);
            }
            TempData.Put("key", tempThemes);
            return true;
        }

            //GET - DOWNLOAD FILE
            public IActionResult GetDownloadedFile(string filePath)
        {
            return FileHelper.DownloadFile(filePath);
        }
    }
}
