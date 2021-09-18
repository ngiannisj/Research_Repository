﻿using Microsoft.AspNetCore.Authorization;
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
            if (actionName == "Add" && id == 0)
            {

                int tempTagId = 1;
                if(tempThemes[0].TagCheckboxes != null && tempThemes[0].TagCheckboxes.Count() > 0)
                {
                    tempTagId = tempThemes[0].TagCheckboxes.Select(u => u.Value).ToList().Max() + 1;
                }

                CheckboxVM tagCheckbox = new CheckboxVM
                {
                    Value = tempTagId,
                    Name = tagName,
                    CheckedState = false
    };
                foreach(ThemeObjectVM tempThemeVM in tempThemes)
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
            }
            else if (actionName == "Delete")
            {
                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    CheckboxVM tagCheckbox = tempThemeVM.TagCheckboxes.FirstOrDefault(u => u.Value == id);
                    tempThemeVM.TagCheckboxes.Remove(tagCheckbox);
                }
            }

            HttpContext.Session.Set("themes", tempThemes);
            IList<Tag> tempTags = tempThemes[0].TagCheckboxes.Select(u => new Tag{ Id = u.Value, Name = u.Name }).ToList();
            IList<ThemeObjectVM> tempThems = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");
            return tempTags;
            }

        //GET - GETTAGNAME (FROM AJAX CALL)
        public string GetTagName(int? id)
        {
            string tagName = "newTag";
            if (id != null)
            {
                IList<ThemeObjectVM> tempThemes = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");
                tagName = tempThemes[0].TagCheckboxes.FirstOrDefault(u => u.Value == id).Name;
            }
            return tagName;
        }
    }
}
