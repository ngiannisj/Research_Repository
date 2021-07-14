using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ItemController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ItemController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Item> objList = _db.Items.Include(u => u.Theme);
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();
            ItemVM itemVM = new ItemVM()
            {
                Item = new Item(),
                ThemeSelectList = _db.Themes.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                TagList = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList()
            };

            if (id == null)
            {
                //Creating
                return View(itemVM);
            }
            else
            {
                //Updating
                itemVM.Item = _db.Items.Find(id);
                if (itemVM.Item == null)
                {
                    return NotFound();
                }
                return View(itemVM);
            }
        }

        private void UpdateItemTagsList(ItemVM itemVM)
        {
            ICollection<int> ThemeTagIdsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == itemVM.Item.ThemeId).Select(i => i.TagId).ToList();
            ICollection<ItemTag> ItemTagsList = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == itemVM.Item.Id).ToList();
            ICollection<int> ItemTagIdsList = ItemTagsList.Select(i => i.TagId).ToList();

            if (itemVM.TagList != null)
            {
                foreach (TagListVM tag in itemVM.TagList)
                {
                    //If the checkbox is checked and the tag is available in the selected theme
                    if (tag.CheckedState && ThemeTagIdsList.Contains(tag.TagId))
                    {
                        //If the entry in the join table does not already exist
                        if (!ItemTagIdsList.Contains(tag.TagId))
                        {
                            _db.ItemTags.Add(new ItemTag
                            {
                                ItemId = itemVM.Item.Id,
                                TagId = tag.TagId
                            });
                        }
                    }
                    else
                    {
                        if (ItemTagIdsList.Contains(tag.TagId))
                        {
                            _db.ItemTags.Remove(ItemTagsList.FirstOrDefault(i => i.TagId == tag.TagId));
                        }
                    }
                }
            }
        }


        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ItemVM itemVM)
        {

            if (ModelState.IsValid)
            {

                if (itemVM.Item.Id == 0)
                {
                    //Creating
                    _db.Items.Add(itemVM.Item);
                    _db.SaveChanges();
                    UpdateItemTagsList(itemVM);
                }
                else
                {
                    //Updating

                    _db.Items.Update(itemVM.Item);
                    UpdateItemTagsList(itemVM);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemVM);
        }


        //DELETE - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Items.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Items.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET - GetAssignedTags
        public ICollection<int> GetAssignedTags(int id)
        {
            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();

            ICollection<int> AssignedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Include(i => i.Tag).Select(i => i.TagId).ToList();

            return AssignedTagIds;
        }
    }
}
