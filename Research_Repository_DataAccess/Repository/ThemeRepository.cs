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
            ICollection<ThemeTag> ThemeTagsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeVM.Theme.Id).ToList();
            ICollection<int> ThemeTagsIdList = ThemeTagsList.Select(i => i.TagId).ToList();

            if (themeVM.TagCheckboxes != null)
            {
                foreach (CheckboxVM tag in themeVM.TagCheckboxes)
                {
                    if (tag.CheckedState)
                    {
                        if (!ThemeTagsIdList.Contains(tag.Value))
                        {
                            //If the tag checkbox has been checked and its previous state was unchecked
                            _db.ThemeTags.Add(new ThemeTag
                            {
                                ThemeId = themeVM.Theme.Id,
                                TagId = tag.Value
                            });
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        //If the tag checkbox is not checked
                        if (ThemeTagsIdList.Contains(tag.Value))
                        {
                            _db.Remove(ThemeTagsList.FirstOrDefault(i => i.TagId == tag.Value));
                        }
                    }
                }
            }
        }

        public void UpdateTagsDb(IList<ThemeObjectVM> tempThemes, IList<Tag> tempTags)
        {
            IList<Tag> dbTagList = _db.Tags.AsNoTracking().ToList();
            IList<int> dbTagIdList = dbTagList.Select(u => u.Id).ToList();

            if (tempThemes != null && tempThemes[0].TagCheckboxes != null && tempThemes.Count > 0)
            {
                if (tempThemes[0].TagCheckboxes.Count > 0)
                {
                    foreach (CheckboxVM tagCheckbox in tempThemes[0].TagCheckboxes)
                    {
                        int index = 0;
                        if (!dbTagIdList.Contains(tagCheckbox.Value))
                        {
                            Tag newTag = new Tag { Id = 0, Name = tagCheckbox.Name };
                            _db.Tags.Add(newTag);
                            _db.SaveChanges();
                            dbTagList = _db.Tags.AsNoTracking().ToList();

                            //Update tags with new ID
                            index = tempThemes[0].TagCheckboxes.IndexOf(tagCheckbox);
                            foreach (ThemeObjectVM themeVM in tempThemes)
                            {
                                themeVM.TagCheckboxes[index].Value = newTag.Id;
                            }
                        }
                        else
                        {
                            Tag tag = _db.Tags.FirstOrDefault(u => u.Id == tagCheckbox.Value);
                            tag.Name = tagCheckbox.Name;
                            _db.Tags.Update(tag);
                        }

                    }

                    IList<int> tagIdListFromThemeVM = new List<int>();
                    if(tempThemes.Count() !=0 )
                    {
                        foreach (CheckboxVM tagCheckbox in tempThemes[0].TagCheckboxes)
                        {
                            tagIdListFromThemeVM.Add(tagCheckbox.Value);
                        }

                        foreach (Tag tag in dbTagList)
                        {
                            if (!tagIdListFromThemeVM.Contains(tag.Id))
                            {
                                _db.Tags.Remove(tag);
                            }
                        }
                    }
                   
                }
            } else
            {
                if (tempTags != null)
                {
                    foreach (Tag tempTag in tempTags)
                    {
                        if (!dbTagIdList.Contains(tempTag.Id))
                        {
                            //Add tags
                            Tag newTag = new Tag { Id = 0, Name = tempTag.Name };
                            _db.Tags.Add(newTag);
                            _db.SaveChanges();
                            dbTagList = _db.Tags.AsNoTracking().ToList();

                            //Update tags with new ID
                            tempTag.Id = newTag.Id;
                        }
                        else
                        {
                            //Update tags
                            Tag tag = _db.Tags.FirstOrDefault(u => u.Id == tempTag.Id);
                            tag.Name = tempTag.Name;
                            _db.Tags.Update(tag);
                        }

                    }

                    //Delete tags
                    IList<int> tempTagIdList = new List<int>();
                    if (tempTags.Count() != 0)
                    {
                        foreach (Tag tempTag in tempTags)
                        {
                            tempTagIdList.Add(tempTag.Id);
                        }

                        foreach (Tag tag in dbTagList)
                        {
                            if (!tempTagIdList.Contains(tag.Id))
                            {
                                _db.Tags.Remove(tag);
                            }
                        }
                    }
                }
            }
        }

        //Get tag checkboxes for theme
        public IList<CheckboxVM> GetTagCheckboxes(int? id, IList<CheckboxVM> tempTagCheckboxes)
        {
            if (id != null)
            {
                ICollection<int> selectedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Select(i => i.TagId).ToList();
                IList<CheckboxVM> tagCheckboxes = _db.Tags.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList();
                if (tempTagCheckboxes.Count() != 0)
                {
                    foreach (CheckboxVM tagCheckbox in tagCheckboxes)
                    {
                        foreach (CheckboxVM tempTagCheckbox in tempTagCheckboxes)
                        {
                            if (tagCheckbox.Value == tempTagCheckbox.Value)
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
                IList<CheckboxVM> tagCheckboxes = _db.Tags.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList();
                return tagCheckboxes;
            }
        }

        public IEnumerable<Tag> GetTags()
        {
            return _db.Tags;
        }

            //Get dropdown list of all tags
            public IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb)
        {
            if (useDb == true)
            {
                tags = _db.Tags.AsNoTracking().ToList();
            }

            if (tags == null)
            {
                tags = new List<Tag>();
            }

            IEnumerable<SelectListItem> tagSelectList = tags.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return tagSelectList;
        }


        public ThemeObjectVM CreateThemeVM(int newId, IList<ThemeObjectVM> themeVMs, string newThemeName, int? id = null)
        {
            ThemeObjectVM themeVM = new ThemeObjectVM();

            IList<CheckboxVM> tempThemeCheckboxes = new List<CheckboxVM>();
            //Get tag checkboxes from themes if they are available
            if (themeVMs != null && themeVMs.Count > 0)
            {
                if (themeVMs[0].TagCheckboxes.Count > 0)
                {
                    tempThemeCheckboxes = themeVMs[0].TagCheckboxes;
                }
            }

            //Assign theme
            if (id == null)
            {
                themeVM.Theme = new Theme();
                themeVM.Theme.Id = newId;
                themeVM.Theme.Name = newThemeName;
                themeVM.TagCheckboxes = GetTagCheckboxes(null, tempThemeCheckboxes);
            }
            else
            {
                themeVM.TagCheckboxes = GetTagCheckboxes(id, tempThemeCheckboxes);
                themeVM.Theme = _db.Themes.AsNoTracking().FirstOrDefault(u => u.Id == id);
            }
            return themeVM;
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
