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

        public void UpdateThemeTagsList(ThemeVM themeVM)
        {
            ICollection<ThemeTag> ThemeTagsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeVM.Theme.Id).ToList();
            ICollection<int> ThemeTagsIdList = ThemeTagsList.Select(i => i.TagId).ToList();

            if (themeVM.TagList != null)
            {
                foreach (TagListVM tag in themeVM.TagList)
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

        public IList<TagListVM> GetTagList(int? id)
        {
            if (id != null)
            {
                ICollection<int> selectedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Select(i => i.TagId).ToList();
                IList<TagListVM> tagList = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList();
                return tagList;
            }else
            {
                IList<TagListVM> tagList = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList();
                return tagList;
            }
        }
        

    public ThemeVM CreateThemeVM(string newId, int? id=null)
        {
            ThemeVM themeVM = new ThemeVM();


            //Assign theme
            if (id == null)
            {
                themeVM.Theme = new Theme();
                themeVM.Theme.Id = Int32.Parse(newId);
                themeVM.TagList = GetTagList(themeVM.Theme.Id);
            }
            else
            {
                themeVM.TagList = GetTagList(id);
                themeVM.Theme = _db.Themes.AsNoTracking().FirstOrDefault(u => u.Id == id);
            }
            return themeVM;
        }

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
