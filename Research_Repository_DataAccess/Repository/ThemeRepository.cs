﻿using Microsoft.AspNetCore.Mvc;
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
        public void UpdateThemeTagsList(ThemeVM themeVM)
        {
            ICollection<ThemeTag> ThemeTagsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeVM.Theme.Id).ToList();
            ICollection<int> ThemeTagsIdList = ThemeTagsList.Select(i => i.TagId).ToList();

            if (themeVM.TagCheckboxes != null)
            {
                foreach (TagListVM tag in themeVM.TagCheckboxes)
                {
                    if (tag.CheckedState)
                    {
                        if (!ThemeTagsIdList.Contains(tag.TagId))
                        {
                            _db.ThemeTags.Add(new ThemeTag
                            {
                                ThemeId = themeVM.Theme.Id,
                                TagId = tag.TagId
                            });
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (ThemeTagsIdList.Contains(tag.TagId))
                        {
                            _db.Remove(ThemeTagsList.FirstOrDefault(i => i.TagId == tag.TagId));
                        }
                    }
                }
            }
        }

        public void UpdateTagsDb(IList<ThemeVM> tempThemes)
        {
            IList<Tag> dbTagList = _db.Tags.AsNoTracking().ToList();
            IList<int> dbTagIdList = dbTagList.Select(u => u.Id).ToList();
            IList<int> tagIdListFromThemeVM = new List<int>();

            foreach (Tag tag in dbTagList)
            {
                if (!tagIdListFromThemeVM.Contains(tag.Id))
                {
                    _db.Tags.Remove(tag);
                }
            }
          
            if(tempThemes.Count > 0 && tempThemes[0].TagCheckboxes != null)
            {
                if (tempThemes[0].TagCheckboxes.Count > 0)
                {
                    foreach (TagListVM tagCheckbox in tempThemes[0].TagCheckboxes)
                    {
                        tagIdListFromThemeVM.Add(tagCheckbox.TagId);
                    }

                    foreach (TagListVM tagCheckbox in tempThemes[0].TagCheckboxes)
                    {
                        int index = 0;
                        if (!dbTagIdList.Contains(tagCheckbox.TagId))
                        {
                            Tag newTag = new Tag { Id = 0, Name = tagCheckbox.Name };
                            _db.Tags.Add(newTag);
                            _db.SaveChanges();

                            //Update tags with new ID
                            index = tempThemes[0].TagCheckboxes.IndexOf(tagCheckbox);
                            foreach (ThemeVM themeVM in tempThemes)
                            {
                                themeVM.TagCheckboxes[index].TagId = newTag.Id;
                            }
                        }
                        else
                        {
                            Tag tag = _db.Tags.FirstOrDefault(u => u.Id == tagCheckbox.TagId);
                            tag.Name = tagCheckbox.Name;
                            _db.Tags.Update(tag);
                        }

                    }
                }
            }
        }

        //Get tag checkboxes for theme
        public IList<TagListVM> GetTagCheckboxes(int? id, IList<TagListVM> tempTagCheckboxes)
        {
            if (id != null)
            {
                ICollection<int> selectedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Select(i => i.TagId).ToList();
                IList<TagListVM> tagCheckboxes = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList();
                if (tempTagCheckboxes.Count() != 0)
                {
                    foreach (TagListVM tagCheckbox in tagCheckboxes)
                    {
                        foreach (TagListVM tempTagCheckbox in tempTagCheckboxes)
                        {
                            if (tagCheckbox.TagId == tempTagCheckbox.TagId)
                            {
                                tagCheckbox.CheckedState = tempTagCheckbox.CheckedState;
                            }
                        }

                    }
                }
                return tagCheckboxes;
            }
            else
            {
                IList<TagListVM> tagCheckboxes = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList();
                return tagCheckboxes;
            }
        }

        //Get dropdown list of all tags
        public IEnumerable<SelectListItem> GetTagList()
        {
            IEnumerable<SelectListItem> tagSelectList = _db.Tags.AsNoTracking().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return tagSelectList;
        }


        public ThemeVM CreateThemeVM(string newId, int? id = null)
        {
            ThemeVM themeVM = new ThemeVM();


            //Assign theme
            if (id == null)
            {
                themeVM.Theme = new Theme();
                themeVM.Theme.Id = Int32.Parse(newId);
                themeVM.TagCheckboxes = GetTagCheckboxes(themeVM.Theme.Id, new List<TagListVM>());
            }
            else
            {
                themeVM.TagCheckboxes = GetTagCheckboxes(id, new List<TagListVM>());
                themeVM.Theme = _db.Themes.AsNoTracking().FirstOrDefault(u => u.Id == id);
            }
            return themeVM;
        }

        //Check if any items are assigned under a theme
        public bool HasItems(int id)
        {
            if (_db.Items.FirstOrDefault(i => i.ThemeId == id) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IList<int> GetThemeIds(IEnumerable<Theme> themes)
        {
            return themes.Select(u => u.Id).ToList();
        }

        public void Update(Theme obj)
        {
            _db.Themes.Update(obj);
        }

        public async Task<IActionResult> DownloadFile(string filePath)
        {
            var path = Directory.GetCurrentDirectory() + "\\wwwroot" + filePath;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            var fileName = Path.GetFileName(path);

            return File(memory, contentType, fileName);
        }
    }
}
