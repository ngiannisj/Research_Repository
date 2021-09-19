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
    public class TagController : Controller
    {

        private readonly ITagRepository _tagRepo;

        public TagController(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        //POST - UPDATE
        public IEnumerable<Tag> UpdateTag(int id, string tagName, string actionName)
        {
            IList<ThemeObjectVM> tempThemes = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");
            IList<Tag> tempTags = HttpContext.Session.Get<IList<Tag>>("tags");
            if (actionName == "Add" && id == 0)
            {

                int tempTagId = 1;
                if( tempThemes != null && tempThemes.Count() != 0 && tempThemes[0].TagCheckboxes != null && tempThemes[0].TagCheckboxes.Count() != 0)
                {
                    tempTagId = tempThemes[0].TagCheckboxes.Select(u => u.Value).ToList().Max() + 1;
                } else
                {
                    tempTagId = HttpContext.Session.Get<IList<Tag>>("tags").Select(u => u.Id).ToList().Max() + 1;
                }

                CheckboxVM tagCheckbox = new CheckboxVM
                {
                    Value = tempTagId,
                    Name = tagName,
                    CheckedState = false
    };
                Tag tag = new Tag { Id = tempTagId, Name = tagName };
                tempTags.Add(tag);

                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    tempThemeVM.TagCheckboxes.Add(tagCheckbox);
                }
            }
            else if (actionName == "Update" && id != 0)
            {
                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    CheckboxVM tagCheckbox = tempThemeVM.TagCheckboxes.FirstOrDefault(u => u.Value == id);
                    tagCheckbox.Name = tagName;
                }

                tempTags.FirstOrDefault(u => u.Id == id).Name = tagName;
            }
            else if (actionName == "Delete")
            {
                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    CheckboxVM tagCheckbox = tempThemeVM.TagCheckboxes.FirstOrDefault(u => u.Value == id);
                    tempThemeVM.TagCheckboxes.Remove(tagCheckbox);
                }

                tempTags.Remove(tempTags.FirstOrDefault(u => u.Id == id));
            }

            HttpContext.Session.Set("themes", tempThemes);
            HttpContext.Session.Set("tags", tempTags);
            IList<ThemeObjectVM> tempThems = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");
            return tempTags;
            }

        //GET - GETTAGNAME (FROM AJAX CALL)
        public string GetTagName(int? id)
        {
            string tagName = "newTag";
            if (id != null)
            {
                IList<Tag> tempTags = HttpContext.Session.Get<IList<Tag>>("tags");
                tagName = tempTags.FirstOrDefault(u => u.Id == id).Name;
            }
            return tagName;
        }
    }
}
