using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository.Models;
using Research_Repository.Models.ViewModels;
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

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ThemeController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Theme> objList = _db.Themes;
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ICollection<int> selectedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Select(i => i.TagId).ToList();
            ThemeVM themeVM = new ThemeVM()
            {
                TagList = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    Checked = selectedTagIds.Contains(i.Id)
                }).ToList()
            };
            if (id == null)
            {
                themeVM.Theme = new Theme();
                //this is for create
                return View(themeVM);
            }
            else
            {
                themeVM.Theme = _db.Themes.AsNoTracking().FirstOrDefault(i => i.Id == id);
                if (themeVM.Theme == null)
                {
                    return NotFound();
                }
                return View(themeVM);
            }
        }


        private void UpdateThemeTagsList(ThemeVM themeVM)
        {
            ICollection<ThemeTag> ThemeTagsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == themeVM.Theme.Id).ToList();
            ICollection<int> ThemeTagsIdList = ThemeTagsList.Select(i => i.TagId).ToList();

            if (themeVM.TagList != null)
            {
                foreach (TagListVM tag in themeVM.TagList)
                {
                    if (tag.Checked)
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
                            _db.ThemeTags.Remove(ThemeTagsList.FirstOrDefault(i => i.TagId == tag.TagId));
                        }
                    }
                }
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ThemeVM obj)
        {

            if (ModelState.IsValid)
            {
                 var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(obj.Theme.Id == 0)
                {
                    //Creating
                    if (files.Count != 0) {
                        //Upload image
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Theme.Image = fileName + extension;
                    }

                    _db.Themes.Add(obj.Theme);
                    _db.SaveChanges();
                    UpdateThemeTagsList(obj);
                } else
                {
                    //Updating
                    var objFromDb = _db.Themes.AsNoTracking().FirstOrDefault(u => u.Id == obj.Theme.Id);

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        if(objFromDb.Image != null)
                        {
                            var oldFile = Path.Combine(upload, objFromDb.Image);

                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Theme.Image = fileName + extension;
                    }
                    else
                    {
                        obj.Theme.Image = objFromDb.Image;
                    }
                    _db.Themes.Update(obj.Theme);
                    UpdateThemeTagsList(obj);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj.Theme);
        }


        //DELETE - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Themes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }else
            {
                if (obj.Image != null)
                {

                    string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

                    var oldFile = Path.Combine(upload, obj.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }
            }
            _db.Themes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
