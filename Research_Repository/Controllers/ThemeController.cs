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
                        themeVMList.Add(_themeRepo.CreateThemeVM(0, themes, theme.Id));
                    }
                    //Update first theme model with the tag select dropdown list
                    if (themeVMList != null && themeVMList.Count > 0)
                    {
                        if(themeVMList[0].TagCheckboxes != null && themeVMList[0].TagCheckboxes.Count() > 0)
                        {
                            IEnumerable<SelectListItem> tagSelectList = themeVMList[0].TagCheckboxes.Select(i => new SelectListItem
                            {
                                Text = i.Name,
                                Value = i.Value.ToString()
                            }).ToList();
                            TempData.Put("tagSelectList", tagSelectList);
                        }
                    }
                    return View(themeVMList);
                }
                else
                {
                    IList<ThemeVM> tempThemes = TempData.Get<IList<ThemeVM>>("themes");
                    TempData.Keep();
                    //Update first theme model with the tag select dropdown list
                    if (tempThemes != null && tempThemes.Count > 0)
                    {
                        if (tempThemes[0].TagCheckboxes != null && tempThemes[0].TagCheckboxes.Count() > 0)
                        {
                            IEnumerable<SelectListItem> tagSelectList = tempThemes[0].TagCheckboxes.Select(i => new SelectListItem
                            {
                                Text = i.Name,
                                Value = i.Value.ToString()
                            }).ToList();
                            TempData.Put("tagSelectList", tagSelectList);
                        }
                    }


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
            TempData.Put("themes", themes);

        }

        public IActionResult SaveThemes(IList<ThemeVM> themes)
        {
                _themeRepo.UpdateTagsDb(themes);

            IList<int> themeIdListFromThemeVM = new List<int>();
            foreach (ThemeVM theme in themes)
            {
                //Get themes from themeVM
                themeIdListFromThemeVM.Add(theme.Theme.Id);
            }

            IEnumerable<Theme> dbThemeList = _themeRepo.GetAll(isTracking: false);
            IList<int> dbThemeIdList = _themeRepo.GetThemeIds(dbThemeList);

            foreach (Theme theme in dbThemeList)
            {
                if (!themeIdListFromThemeVM.Contains(theme.Id))
                {
                    _themeRepo.Remove(theme);
                    _themeRepo.Save();
                }
            }
            foreach (ThemeVM theme in themes)
            {

                if (dbThemeIdList.Contains(theme.Theme.Id))
                {
                    _themeRepo.UpdateThemeTagsList(theme);
                    _themeRepo.Update(theme.Theme);
                    _themeRepo.Save();
                }
                else
                {
                    theme.Theme.Id = 0;
                    _themeRepo.Add(theme.Theme);
                    _themeRepo.Save();
                    _themeRepo.UpdateThemeTagsList(theme);
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
            int newId = 1;
            if (themes.Count > 0)
            {
                newId = themes[themes.Count - 1].Theme.Id + 1;
            }
            themes.Add(_themeRepo.CreateThemeVM(newId, themes, null));
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveThemesState(themes);
            return RedirectToAction("Index", new { redirect = true });
        }

            //GET - DOWNLOAD FILE
            public IActionResult GetDownloadedFile(string filePath)
        {
            return FileHelper.DownloadFile(filePath);
        }
    }
}
