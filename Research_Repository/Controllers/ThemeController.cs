using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.LibrarianRole)]
    public class ThemeController : Controller
    {

        private readonly IThemeRepository _themeRepo;

        public ThemeController(IThemeRepository themeRepo)
        {
            _themeRepo = themeRepo;
        }

        public IActionResult Index(bool redirect)
        {

            //If theme page is loaded not due to theme session changes, and loads data from database not session
            if (redirect == false)
            {
                //Get list of themes from database
                IList<Theme> themeList = _themeRepo.GetAll(isTracking: false).ToList();

                //Populate a list of theme view models to display in the view
                IList<ThemeObjectVM> themeVMList = new List<ThemeObjectVM>();
                if (themeList != null && themeList.Count > 0)
                {
                    foreach (Theme theme in themeList)
                    {
                        //Create a themeVM for each theme and add it to themeVMList
                        themeVMList.Add(_themeRepo.CreateThemeVM(0, null, null, theme.Id));
                    }
                }

                //Update session with themes and tags
                SaveThemesState(themeVMList);

                //Populate a theme view model object to pass to the view
                ThemeVM themeVMObject = new ThemeVM { ThemeObjects = themeVMList, SelectedThemeName = "", SelectedThemeDescription = "" };

                return View(themeVMObject);
            }

            //If theme page loads because the theme state in session has updated
            else
            {
                //Get temp themes from session
                IList<ThemeObjectVM> tempThemes = HttpContext.Session.Get<IList<ThemeObjectVM>>(WC.SessionThemes);

                //Solves error where inputs in the view display the incorrect values
                ModelState.Clear();

                //Populate a theme view model object to pass to the view
                ThemeVM themeVMObject = new ThemeVM { ThemeObjects = tempThemes, SelectedThemeName = "", SelectedThemeDescription = "" };

                return View(themeVMObject);
            }
        }

        //Save themes to database from temp themes in session
        public IActionResult SaveThemes(string themeVMString)
        {
            IList<ThemeObjectVM> themeObjects = JsonConvert.DeserializeObject<IList<ThemeObjectVM>>(themeVMString);

            //Update tags in database
            _themeRepo.UpdateTagsDb(HttpContext.Session.Get<IList<Tag>>(WC.SessionTags));

            //Get a list of theme ids from themes returned from view
            IList<int> tempThemeIdList = new List<int>();
            //If the themes list returned from the view is not null or empty
            if (themeObjects != null && themeObjects.Count > 0)
            {
                foreach (ThemeObjectVM theme in themeObjects)
                {
                    //Get themes ids from themes returned from view
                    tempThemeIdList.Add(theme.Theme.Id);
                }


                //Get themes from database
                IEnumerable<Theme> dbThemeList = _themeRepo.GetAll(isTracking: false);
                //Get a list of theme ids from themes in the database
                IList<int> dbThemeIdList = _themeRepo.GetThemeIds(dbThemeList);

                //Remove themes from database if they do not exist in the themes returned from the view
                foreach (Theme theme in dbThemeList)
                {
                    if (!tempThemeIdList.Contains(theme.Id))
                    {
                        _themeRepo.Remove(theme);
                        _themeRepo.Save();
                    }
                }

                //Add/Update themes in database
                foreach (ThemeObjectVM theme in themeObjects)
                {
                    //If theme exists in database
                    if (dbThemeIdList.Contains(theme.Theme.Id))
                    {
                        //Update tag checkbox state for this theme
                        _themeRepo.UpdateThemeTagsList(theme);

                        //Update this theme in the database
                        _themeRepo.Update(theme.Theme);
                        _themeRepo.Save();
                    }

                    //If theme does not exist in the database
                    else
                    {
                        //Set theme id to '0' so a unique value is assigned in the database
                        theme.Theme.Id = 0;
                        //Add theme to database
                        _themeRepo.Add(theme.Theme);
                        _themeRepo.Save();
                        //Update tag checkbox state for this theme. This task is completed after adding the theme to the database so the theme id is updated beforehand.
                        _themeRepo.UpdateThemeTagsList(theme);
                    }
                }
            }

            _themeRepo.Save();

            //Solves error where inputs in the view display the incorrect values
            ModelState.Clear();
            return RedirectToAction(nameof(Index));
        }


        //Delete theme from session
        public void DeleteTheme(string themeVMString, int deleteId)
        {
            IList<ThemeObjectVM> themeObjects = JsonConvert.DeserializeObject<IList<ThemeObjectVM>>(themeVMString);
            //Get theme from themes returned from view
            ThemeObjectVM themeToRemove = new ThemeObjectVM();
            if (themeObjects != null && themeObjects.Count > 0)
            {
                themeToRemove = themeObjects.FirstOrDefault(u => u.Theme.Id == deleteId);

                //Remove theme from temp themes in session
                themeObjects.Remove(themeToRemove);
            }

            //Solves error where inputs in the view display the incorrect values
            ModelState.Clear();

            //Update theme session values
            SaveThemesState(themeObjects);
        }

        //Add new theme to session themes
        public void AddTheme(string themeVMString, string themeName, string themeDesc)
        {
            IList<ThemeObjectVM> themeObjects = JsonConvert.DeserializeObject<IList<ThemeObjectVM>>(themeVMString);

            //Generate new unique id for the new team
            int newId = 1;
            //If teams exist in the teams list returned from the view, find the largest id number and add 1 to it to find the next unique id number
            if (themeObjects != null && themeObjects.Count > 0)
            {
                newId = themeObjects.Select(u => u.Theme.Id).ToList().Max() + 1;
            }

            //Instantiate an empty themes list if a list does not exist
            if (themeObjects == null)
            {
                themeObjects = new List<ThemeObjectVM>();
            }

            //Add new theme to temp themes object
            themeObjects.Add(_themeRepo.CreateThemeVM(newId, themeName, themeDesc, null));

            //Solves error where inputs in the view display the incorrect values
            ModelState.Clear();

            //Update session with updated tempThemes
            SaveThemesState(themeObjects);
        }

        //Save theme/tag/tagSelectList state to session
        public void SaveThemesState([FromBody] IList<ThemeObjectVM> themes)
        {
            //Set themes in session
            HttpContext.Session.Set(WC.SessionThemes, themes);

            //If no tags exist in session
            if (HttpContext.Session.Get<IList<Tag>>(WC.SessionTags) == null)
            {
                //Get list of tags from database
                IList<Tag> tags = _themeRepo.GetTags().ToList();

                //Set tags from database into session
                HttpContext.Session.Set(WC.SessionTags, tags);
            }

            //Set tagSelectList in session if it does not exist (Should only be required on page load before updates have been made to the tags)
            if (TempData.Get<IList<SelectListItem>>(WC.TempDataTagSelectList) == null)
            {
                //Get list of tags from database
                IList<Tag> tags = _themeRepo.GetTags().ToList();

                //Update tempdata with the an updated tag select dropdown list
                IEnumerable<SelectListItem> tagSelectList = _themeRepo.GetTagList(tags, false);
                TempData.Put(WC.TempDataTagSelectList, tagSelectList);
            }
        }
    }

}
