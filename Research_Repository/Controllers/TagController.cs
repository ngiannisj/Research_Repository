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

        public IActionResult Index()
        {
            IEnumerable<Tag> objList = _tagRepo.GetAll();
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
                tag = _tagRepo.Find(id.GetValueOrDefault());
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
                    _tagRepo.Add(tag);
                }
                else
                {
                    //Updating
                    _tagRepo.Update(tag);
                }

                _tagRepo.Save();
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
            var tag = _tagRepo.Find(id.GetValueOrDefault());
            if (tag == null)
            {
                return NotFound();
            }
            _tagRepo.Remove(tag);
            _tagRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
