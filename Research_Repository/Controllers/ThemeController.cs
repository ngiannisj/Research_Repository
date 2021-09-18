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

        public IActionResult Index(ThemeVM themeVM, bool redirect)
        {
            IEnumerable<Theme> themeList = _themeRepo.GetAll(isTracking: false);

            if (themeVM.ThemeObjects == null || themeVM.ThemeObjects.Count == 0)
            {
                if (redirect == false)
                {
                    //If no form button is clicked
                    IList<ThemeObjectVM> themeVMList = new List<ThemeObjectVM>();
                    foreach (Theme theme in themeList)
                    {
                        themeVMList.Add(_themeRepo.CreateThemeVM(0, themeVM.ThemeObjects, null, theme.Id));
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

                    ThemeVM themeVMObject = new ThemeVM { ThemeObjects = themeVMList, NewThemeName = "" };
                    return View(themeVMObject);
                }
                else
                {
                    IList<ThemeObjectVM> tempThemes = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");
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
                    ThemeVM themeVMObject = new ThemeVM { ThemeObjects = tempThemes, NewThemeName = "" };
                    return View(themeVMObject);
                }
            }
            else
            {
                ThemeVM themeVMObject = new ThemeVM { ThemeObjects = themeVM.ThemeObjects, NewThemeName = "" };

                return View(themeVMObject);
            };
        }

        public void SaveThemesState([FromBody]IList<ThemeObjectVM> themes)
        {
            HttpContext.Session.Set("themes", themes);

        }

        public IActionResult SaveThemes(ThemeVM themeVM)
        {
                _themeRepo.UpdateTagsDb(themeVM.ThemeObjects);

            IList<int> themeIdListFromThemeVM = new List<int>();
            if(themeVM.ThemeObjects != null && themeVM.ThemeObjects.Count > 0)
            {
                foreach (ThemeObjectVM theme in themeVM.ThemeObjects)
                {
                    //Get themes from themeVM
                    themeIdListFromThemeVM.Add(theme.Theme.Id);
                }
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
            if (themeVM.ThemeObjects != null && themeVM.ThemeObjects.Count > 0)
            {
                foreach (ThemeObjectVM theme in themeVM.ThemeObjects)
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

                }
            }
            _themeRepo.Save();
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            return RedirectToAction("Index");
        }



        public IActionResult DeleteTheme(ThemeVM themeVM, int deleteId)
        {
            ThemeObjectVM itemToRemove = themeVM.ThemeObjects.FirstOrDefault(u => u.Theme.Id == deleteId);
            if (!_themeRepo.HasItems(itemToRemove.Theme.Id))
            {
                themeVM.ThemeObjects.Remove(itemToRemove);
            }
            else
            {
                //Give warning to not delete until items are removed
            }
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveThemesState(themeVM.ThemeObjects);
            return RedirectToAction("Index", new { redirect = true });
        }

        public IActionResult AddTheme(ThemeVM themeVM)
        {
            int newId = 1;
            if (themeVM.ThemeObjects != null && themeVM.ThemeObjects.Count > 0)
            {
                newId = themeVM.ThemeObjects[themeVM.ThemeObjects.Count - 1].Theme.Id + 1;
            } else
            {
                themeVM.ThemeObjects = new List<ThemeObjectVM>();
            }
            themeVM.ThemeObjects.Add(_themeRepo.CreateThemeVM(newId, themeVM.ThemeObjects, themeVM.NewThemeName, null));
            ModelState.Clear(); //Solves error where inputs in the view display the incorrect values
            SaveThemesState(themeVM.ThemeObjects);
            return RedirectToAction("Index", new { redirect = true });
        }

            //GET - DOWNLOAD FILE
            public IActionResult GetDownloadedFile(string filePath)
        {
            return FileHelper.DownloadFile(filePath);
        }
    }
}
