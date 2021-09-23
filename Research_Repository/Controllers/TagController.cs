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
        //Update 'tempTags' object in session with added/updated/deleted tag
        public IEnumerable<Tag> UpdateTag(int id, string tagName, string actionName)
        {
            //Get list of temp tags from the session
            IList<Tag> tempTags = HttpContext.Session.Get<IList<Tag>>("tags");
            //Get list of temp themes from the session
            IList<ThemeObjectVM> tempThemes = HttpContext.Session.Get<IList<ThemeObjectVM>>("themes");

            //Adding a new tag
            if (actionName == "Add" && id == 0)
            {
                //Generate a unique id for a newly added tag
                int newId = 1;
                if(tempTags != null && tempTags.Count > 0)
                {
                    //Get next available id for the tag if tags exist in 'tempTags'
                    newId = HttpContext.Session.Get<IList<Tag>>("tags").Select(u => u.Id).ToList().Max() + 1;
                }
                else
                {
                    //If no tags exist in 'tempTeams' get next available id value from database
                    IEnumerable<int> tagList = _tagRepo.GetAll(isTracking: false).Select(u => u.Id);
                    if (tagList.Count() > 0)
                    {
                        newId = tagList.Last() + 1;
                    }
                    //If no projects exist in database value of 'newId' stays at 1
                }

                //Create new tag
                Tag tag = new Tag { Id = newId, Name = tagName };
                //Add new tag to 'tempTags' list
                tempTags.Add(tag);

                //If any themes exist in 'tempThemes' add a new tag checkbox to each theme
                if(tempThemes != null && tempThemes.Count > 0)
                {
                    //Create tag Checkbox
                    CheckboxVM tagCheckbox = new CheckboxVM
                    {
                        Value = newId,
                        Name = tagName,
                        CheckedState = false
                    };
                    //Add tag checkbox to every theme in 'tempThemes'
                    foreach (ThemeObjectVM tempThemeVM in tempThemes)
                    {
                        tempThemeVM.TagCheckboxes.Add(tagCheckbox);
                    }
                }
            }

            //Updating an existing tag
            else if (actionName == "Update")
            {
                //Update the name of the tag in 'tempTags'
                tempTags.FirstOrDefault(u => u.Id == id).Name = tagName;

                //Update the name of the tag checkbox in every theme in 'tempThemes'
                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    CheckboxVM tagCheckbox = tempThemeVM.TagCheckboxes.FirstOrDefault(u => u.Value == id);
                    tagCheckbox.Name = tagName;
                }
            }

            //Deleting a tag
            else if (actionName == "Delete")
            {
                //Remove the tag from'tempTags'
                tempTags.Remove(tempTags.FirstOrDefault(u => u.Id == id));

                //Remove the tag checkbox from every theme in 'tempThemes' for the selected tag
                foreach (ThemeObjectVM tempThemeVM in tempThemes)
                {
                    CheckboxVM tagCheckbox = tempThemeVM.TagCheckboxes.FirstOrDefault(u => u.Value == id);
                    tempThemeVM.TagCheckboxes.Remove(tagCheckbox);
                }
            }

            //Update 'tempTags' in session
            HttpContext.Session.Set("tags", tempTags);
            //Update 'tempThemes' in session
            HttpContext.Session.Set("themes", tempThemes);

            return tempTags;
            }

        //GET - GETTAGNAME (FROM AJAX CALL)
        public string GetTagName(int? id)
        {
            //Set default tag name to 'newTag' for newly added tags
            string tagName = "newTag";

            //If tag already exists
            if (id != null)
            {
                //Get list of 'tempTags' form session
                IList<Tag> tempTags = HttpContext.Session.Get<IList<Tag>>("tags");
                //Assign name of selected tag to 'tagName' to be returned
                tagName = tempTags.FirstOrDefault(u => u.Id == id).Name;
            }
            return tagName;
        }
    }
}
