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
            IEnumerable<Item> objList = _db.Item.Include(u => u.Theme);
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ItemVM itemVM = new ItemVM()
            {
                Item = new Item(),
                ThemeSelectList = _db.Theme.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                //Creating
                return View(itemVM);
            }
            else
            {
                //Updating
                itemVM.Item = _db.Item.Find(id);
                if (itemVM.Item == null)
                {
                    return NotFound();
                }
                return View(itemVM);
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
                    _db.Item.Add(itemVM.Item);
                }
                else
                {
                    //Updating
                    _db.Item.Update(itemVM.Item);
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
            var obj = _db.Item.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Item.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
