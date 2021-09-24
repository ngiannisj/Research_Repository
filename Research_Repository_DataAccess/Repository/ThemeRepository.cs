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
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class ThemeRepository : Repository<Theme>, IThemeRepository
    {
        private readonly ApplicationDbContext _db;

        public ThemeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Updates tags assigned to themes
        public void UpdateThemeTagsList(ThemeObjectVM themeVM)
        {
            //Generate list of tags available for the theme returned in the parameter (From database)
            ICollection<ThemeTag> ThemeTagsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeVM.Theme.Id).ToList();

            //Generate list of tag ids from tags in 'ThemeTagsList'
            ICollection<int> ThemeTagsIdList = ThemeTagsList.Select(i => i.TagId).ToList();

            //If checkboxes are returned from 'themeVM'
            if (themeVM.TagCheckboxes != null)
            {
                //For each tag checkbox in themeVM
                foreach (CheckboxVM tagCheckbox in themeVM.TagCheckboxes)
                {
                    //If tag checkbox is checked
                    if (tagCheckbox.CheckedState)
                    {
                        //If theme tag relationship does not already exist in themeTags database join table (If the tagCheckbox has been checked and its previous state was unchecked)
                        if (!ThemeTagsIdList.Contains(tagCheckbox.Value))
                        {
                            //Add theme tag relationship to database
                            _db.ThemeTags.Add(new ThemeTag
                            {
                                ThemeId = themeVM.Theme.Id,
                                TagId = tagCheckbox.Value
                            });

                            _db.SaveChanges();
                        }
                    }
                    //If the tag checkbox is not checked
                    else
                    {
                        //If theme tag relationship exists in themeTags database join table, remove the relationship
                        if (ThemeTagsIdList.Contains(tagCheckbox.Value))
                        {
                            _db.Remove(ThemeTagsList.FirstOrDefault(i => i.TagId == tagCheckbox.Value));
                        }
                    }
                }
            }
        }

        public void UpdateTagsDb(IList<Tag> tempTags)
        {
            //Get list of tags from database
            IList<Tag> dbTagList = _db.Tags.AsNoTracking().ToList();
            //Get list of tag ids from database
            IList<int> dbTagIdList = dbTagList.Select(u => u.Id).ToList();

            //If tempTags list from session is not empty
            if (tempTags != null && tempTags.Count > 0)
            {
                foreach (Tag tempTag in tempTags)
                {
                    //If tempTag does not exist in database tag table
                    if (!dbTagIdList.Contains(tempTag.Id))
                    {
                        //Set id of tag to '0' to ensure it receives a unique id when added to the database, and instantiate a new tag object
                        Tag newTag = new Tag { Id = 0, Name = tempTag.Name };

                        //Add new tag to database
                        _db.Tags.Add(newTag);
                        _db.SaveChanges();

                        //Update 'dbTagList' with new tag value
                        dbTagList = _db.Tags.AsNoTracking().ToList();

                        //Update new tag with new ID
                        tempTag.Id = newTag.Id;
                    }

                    //If temp tag already exists in database, update it
                    else
                    {
                        //Get tag from database
                        Tag tag = _db.Tags.FirstOrDefault(u => u.Id == tempTag.Id);
                        //Update tag name
                        tag.Name = tempTag.Name;

                        //Update tag in database
                        _db.Tags.Update(tag);

                        //Update 'dbTagList' with updated tag value
                        dbTagList = _db.Tags.AsNoTracking().ToList();

                    }

                }

                //Get tempTagIds from temp tags
                IList<int> tempTagIds = tempTags.Select(u => u.Id).ToList();

                //If tempTags is not empty
                if (tempTags.Count > 0)
                {
                    foreach (Tag dbTag in dbTagList)
                    {
                        //If database tag does not exist in tempTags, remove it from the database
                        if (!tempTagIds.Contains(dbTag.Id))
                        {
                            _db.Tags.Remove(dbTag);
                        }
                    }
                }
            }
        }


        public ThemeObjectVM CreateThemeVM(int newId, IList<ThemeObjectVM> themeVMs, string newThemeName, int? id = null)
        {
            //Instantiate themeVM
            ThemeObjectVM themeVM = new ThemeObjectVM();

            //If no theme is selected, generate a new theme
            if (id == null)
            {
                themeVM.Theme = new Theme();
                themeVM.Theme.Id = newId;
                themeVM.Theme.Name = newThemeName;
                //Generate checkboxes for tags
                themeVM.TagCheckboxes = GetTagCheckboxes(null);
            }
            //If an existing theme is selected
            else
            {
                //Get checkboxes for tags
                themeVM.TagCheckboxes = GetTagCheckboxes(id);
                //Get theme
                themeVM.Theme = _db.Themes.AsNoTracking().FirstOrDefault(u => u.Id == id);
            }

            return themeVM;
        }

        //Get tag checkboxes for theme
        public IList<CheckboxVM> GetTagCheckboxes(int? themeId)
        {
            //Instantiate list of tags
            IList<Tag> tagsList = new List<Tag>();
            //If session object is not null
            if(HttpContext != null && HttpContext.Session.Get<IList<Tag>>("tags") != null)
            {
                tagsList = HttpContext.Session.Get<IList<Tag>>("tags");
            }
            //If tags session object is null, populate 'tagsList' with tags from database
            else
            {
                tagsList = _db.Tags.AsNoTracking().ToList();
            }

            //If a theme id is provided in parameter
            if (themeId != null)
            {
                //Get tags with a theme relationship in the 'ThemeTags' join table in the database
                ICollection<int> selectedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeId).Select(i => i.TagId).ToList();

                //Generate tag checkboxes with tagsList
                IList<CheckboxVM> tagCheckboxes = tagsList.Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id) //If tag has relationship with the selected themeId, set CheckedState to true. Else set CheckedState to false
                }).ToList();

                return tagCheckboxes;
            }

            //If themeId is not provided
            else
            {
                //Generate unchecked tag checkboxes from tags in session
                IList<CheckboxVM> tagCheckboxes = tagsList.Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList();
                return tagCheckboxes;
            }
        }

        //Get all tags from database
        public IEnumerable<Tag> GetTags()
        {
            return _db.Tags;
        }

        //Generate tag dropdown selectlist
        public IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb)
        {
            //If 'useDb' parameter is set to true, get tags list from database
            if (useDb == true)
            {
                tags = _db.Tags.AsNoTracking().ToList();
            }

            //If tags is null, instantiate an empty tags list
            if (tags == null)
            {
                tags = new List<Tag>();
            }

            //Generate tags dropdown selectlist
            IEnumerable<SelectListItem> tagSelectList = tags.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return tagSelectList;
        }

        public IList<int> GetThemeIds(IEnumerable<Theme> themes)
        {
            return themes.Select(u => u.Id).ToList();
        }

        public void Update(Theme obj)
        {
            _db.Themes.Update(obj);
        }
    }
}
