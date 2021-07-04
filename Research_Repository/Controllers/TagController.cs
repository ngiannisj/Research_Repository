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
    public class TagController : Controller
    {

        private readonly ApplicationDbContext _db;

        public TagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Tag> objList = _db.Tags;
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            Tag tag = new Tag();
            if (id == null)
            {
                //Creating
                return View(tag);
            }
            else
            {
                //Updating
                tag = _db.Tags.Find(id);
                if (tag == null)
                {
                    return NotFound();
                }
                return View(tag);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Tag tag)
        {

            if (ModelState.IsValid)
            {

                if (tag.Id == 0)
                {
                    //Creating
                    _db.Tags.Add(tag);
                }
                else
                {
                    //Updating
                    _db.Tags.Update(tag);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tag);
        }


        //DELETE - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var tag = _db.Tags.Find(id);
            if (tag == null)
            {
                return NotFound();
            }
            _db.Tags.Remove(tag);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
